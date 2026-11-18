using System.Text.Json;
using TaxCalculatorAPI.Models;
using TaxCalculatorAPI.Models.Requests;
using TaxCalculatorAPI.Models.Responses;
using TaxCalculatorAPI.Services.Interfaces;
using TaxCalculatorAPI.Validation;

namespace TaxCalculatorAPI.Services;

public sealed class TaxCalculatorService : ITaxComparisonService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<TaxCalculatorService> _logger;
    private readonly CompareTaxRequestValidator _validator;
    private readonly Dictionary<string, TaxRules> _rulesCache = new(StringComparer.OrdinalIgnoreCase);

    public TaxCalculatorService(IWebHostEnvironment environment, ILogger<TaxCalculatorService> logger, CompareTaxRequestValidator validator)
    {
        _environment = environment;
        _logger = logger;
        _validator = validator;
        _ = LoadRules("2026-27");
    }

    public OperationResult<CompareTaxResponse> Compare(CompareTaxRequest request)
    {
        try
        {
            var errors = _validator.Validate(request);
            if (errors.Count > 0)
                return OperationResult<CompareTaxResponse>.ValidationFailure(string.Join(" ", errors.SelectMany(e => e.Value)));

            var rules = LoadRules(request.TaxYear);
            return OperationResult<CompareTaxResponse>.Success(CompareRegimes(request, rules));
        }
        catch (KeyNotFoundException ex)
        {
            return OperationResult<CompareTaxResponse>.ValidationFailure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Tax comparison failed for tax year {TaxYear}.", request.TaxYear);
            return OperationResult<CompareTaxResponse>.Failure("An unexpected error occurred while calculating tax.");
        }
    }

    private CompareTaxResponse CompareRegimes(CompareTaxRequest request, TaxRules rules)
    {
        var oldResult = CalculateOldRegime(request, rules);
        var newResult = CalculateNewRegime(request, rules);
        return new CompareTaxResponse
        {
            TaxYear = rules.TaxYear,
            OldRegime = oldResult,
            NewRegime = newResult,
            RecommendedRegime = oldResult.FinalTax < newResult.FinalTax ? "Old" : newResult.FinalTax < oldResult.FinalTax ? "New" : "Same",
            Savings = Math.Abs(oldResult.FinalTax - newResult.FinalTax)
        };
    }

    private RegimeResultResponse CalculateOldRegime(CompareTaxRequest request, TaxRules rules)
    {
        var regime = rules.Regimes["old"];
        var gross = CalculateGrossTotalIncome(request);
        var section80C = Math.Min(request.OldRegimeDeductions.Section80C, regime.Section80CLimit);
        var section80D = Math.Min(request.OldRegimeDeductions.Section80D, regime.Section80DLimit);
        var otherDeductions = Math.Min(request.OldRegimeDeductions.OtherDeductions, regime.OtherDeductionsLimit);
        var deductions = section80C + section80D + otherDeductions;
        var taxable = Math.Max(0, gross - regime.StandardDeduction - deductions);
        var slabTax = CalculateSlabTax(taxable, regime.AgeCategories[request.AgeCategory.ToLowerInvariant()]);
        return CreateRegimeResult(gross, regime.StandardDeduction, deductions, taxable, slabTax, regime, rules.CessRate, []);
    }

    private RegimeResultResponse CalculateNewRegime(CompareTaxRequest request, TaxRules rules)
    {
        var regime = rules.Regimes["new"];
        var gross = CalculateGrossTotalIncome(request);
        var warnings = new List<string>();
        var deductions = 0m;
        if (request.NewRegimeDeductions > 0 && regime.AllowsNewRegimeDeductions)
            deductions = request.NewRegimeDeductions;
        else if (request.NewRegimeDeductions > 0)
            warnings.Add("New regime deductions are not allowed by the configured rules and were ignored.");
        var taxable = Math.Max(0, gross - regime.StandardDeduction - deductions);
        var slabTax = CalculateSlabTax(taxable, regime.AgeCategories[request.AgeCategory.ToLowerInvariant()]);
        return CreateRegimeResult(gross, regime.StandardDeduction, deductions, taxable, slabTax, regime, rules.CessRate, warnings);
    }

    private static decimal CalculateGrossTotalIncome(CompareTaxRequest request) =>
        Math.Max(0, request.GrossSalary - request.SalaryAllowancesExemptBothRegimes + request.OtherIncome - request.HousePropertyInterest);

    private static decimal CalculateSlabTax(decimal income, IReadOnlyList<TaxSlab> slabs)
    {
        decimal tax = 0, lower = 0;
        foreach (var slab in slabs)
        {
            var upper = slab.UpTo == 0 ? income : Math.Min(income, slab.UpTo);
            if (upper > lower) tax += (upper - lower) * slab.Rate;
            if (slab.UpTo == 0 || income <= slab.UpTo) break;
            lower = slab.UpTo;
        }
        return tax;
    }

    private static RegimeResultResponse CreateRegimeResult(decimal gross, decimal standardDeduction, decimal deductions, decimal taxable,
        decimal slabTax, RegimeRules rules, decimal cessRate, List<string> warnings)
    {
        var rebate = taxable <= rules.RebateThreshold ? Math.Min(slabTax, rules.MaximumRebate) : 0;
        var afterRebate = Math.Max(0, slabTax - rebate);
        const decimal surcharge = 0;
        warnings.Add("Surcharge is set to zero for this simplified calculation.");
        var cess = (afterRebate + surcharge) * cessRate;
        return new RegimeResultResponse
        {
            GrossTotalIncome = gross,
            StandardDeduction = standardDeduction,
            TotalDeductions = standardDeduction + deductions,
            TaxableIncome = taxable,
            SlabTax = slabTax,
            Rebate87A = rebate,
            TaxAfterRebate = afterRebate,
            Surcharge = surcharge,
            HealthAndEducationCess = cess,
            FinalTax = Math.Round(afterRebate + surcharge + cess, 0, MidpointRounding.AwayFromZero),
            Warnings = warnings
        };
    }

    private TaxRules LoadRules(string taxYear)
    {
        if (_rulesCache.TryGetValue(taxYear, out var cached)) return cached;
        var path = Path.Combine(_environment.ContentRootPath, "Rules", $"tax-rules-{taxYear}.json");
        if (!File.Exists(path)) throw new KeyNotFoundException($"Tax rules for year '{taxYear}' are not supported (expected file '{path}').");

        TaxRules rules;
        try
        {
            rules = JsonSerializer.Deserialize<TaxRules>(File.ReadAllText(path), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? throw new InvalidDataException("JSON root is empty.");
        }
        catch (Exception ex)
        {
            throw new InvalidDataException($"Tax rules file '{path}' is invalid: {ex.Message}", ex);
        }

        if (rules.TaxYear != taxYear || string.IsNullOrWhiteSpace(rules.VerifiedAgainst) || rules.CessRate < 0 ||
            rules.Regimes.Count != 2 || !rules.Regimes.ContainsKey("old") || !rules.Regimes.ContainsKey("new"))
            throw new InvalidDataException($"Tax rules file '{path}' is invalid: required metadata or regimes are missing.");
        foreach (var (name, regime) in rules.Regimes)
        {
            if (regime.AgeCategories.Count != 3 ||
                !regime.AgeCategories.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase).SetEquals(["below60", "60to79", "80plus"]) ||
                regime.AgeCategories.Values.Any(slabs => slabs.Count == 0 || slabs[^1].UpTo != 0 || slabs.Any(s => s.Rate < 0 || s.UpTo < 0)))
                throw new InvalidDataException($"Tax rules file '{path}' has invalid slabs for regime '{name}'.");
        }
        _rulesCache[taxYear] = rules;
        return rules;
    }
}
