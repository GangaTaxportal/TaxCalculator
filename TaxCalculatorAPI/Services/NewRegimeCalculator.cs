using TaxCalculatorAPI.Models;
using TaxCalculatorAPI.Models.Requests;
using TaxCalculatorAPI.Models.Responses;
namespace TaxCalculatorAPI.Services;
public sealed class NewRegimeCalculator(SlabCalculator slabs)
{
    public RegimeResultResponse Calculate(CompareTaxRequest request, TaxRules rules)
    {
        var r = rules.Regimes["new"]; var gross = Math.Max(0, request.GrossSalary - request.SalaryAllowancesExemptBothRegimes + request.OtherIncome - request.HousePropertyInterest);
        var warnings = new List<string>(); decimal deductions = 0;
        if (request.NewRegimeDeductions > 0 && r.AllowsNewRegimeDeductions) deductions = request.NewRegimeDeductions;
        else if (request.NewRegimeDeductions > 0) warnings.Add("New regime deductions are not allowed by the configured rules and were ignored.");
        var taxable = Math.Max(0, gross - r.StandardDeduction - deductions); var slabTax = slabs.Calculate(taxable, r.AgeCategories[request.AgeCategory.ToLowerInvariant()]);
        return TaxResultFactory.Create(gross, r.StandardDeduction, deductions, taxable, slabTax, r, rules.CessRate, warnings);
    }
}
internal static class TaxResultFactory
{
    public static RegimeResultResponse Create(decimal gross, decimal standard, decimal deductions, decimal taxable, decimal slabTax, RegimeRules rules, decimal cessRate, List<string> warnings)
    {
        var rebate = taxable <= rules.RebateThreshold ? Math.Min(slabTax, rules.MaximumRebate) : 0;
        var after = Math.Max(0, slabTax - rebate); const decimal surcharge = 0;
        warnings.Add("Surcharge is set to zero for this simplified calculation.");
        var cess = (after + surcharge) * cessRate;
        return new() { GrossTotalIncome = gross, StandardDeduction = standard, TotalDeductions = standard + deductions, TaxableIncome = taxable, SlabTax = slabTax, Rebate87A = rebate, TaxAfterRebate = after, Surcharge = surcharge, HealthAndEducationCess = cess, FinalTax = Math.Round(after + surcharge + cess, 0, MidpointRounding.AwayFromZero), Warnings = warnings };
    }
}
