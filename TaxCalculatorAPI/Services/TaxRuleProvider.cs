using System.Text.Json;
using TaxCalculatorAPI.Models;
namespace TaxCalculatorAPI.Services;
public sealed class TaxRuleProvider
{
    private readonly string _directory;
    private readonly Dictionary<string, TaxRules> _cache = new(StringComparer.OrdinalIgnoreCase);
    public TaxRuleProvider(IWebHostEnvironment env) => _directory = Path.Combine(env.ContentRootPath, "Rules");
    public TaxRules Get(string year)
    {
        if (_cache.TryGetValue(year, out var cached)) return cached;
        var path = Path.Combine(_directory, $"tax-rules-{year}.json");
        if (!File.Exists(path)) throw new KeyNotFoundException($"Tax rules for year '{year}' are not supported (expected file '{path}').");
        TaxRules rules;
        try { rules = JsonSerializer.Deserialize<TaxRules>(File.ReadAllText(path), new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? throw new InvalidDataException("JSON root is empty."); }
        catch (Exception ex) { throw new InvalidDataException($"Tax rules file '{path}' is invalid: {ex.Message}", ex); }
        if (rules.TaxYear != year || string.IsNullOrWhiteSpace(rules.VerifiedAgainst) || rules.CessRate < 0 || rules.Regimes.Count != 2 || !rules.Regimes.ContainsKey("old") || !rules.Regimes.ContainsKey("new")) throw new InvalidDataException($"Tax rules file '{path}' is invalid: required metadata or regimes are missing.");
        foreach (var (name, regime) in rules.Regimes)
            if (regime.AgeCategories.Count != 3 || !regime.AgeCategories.Keys.ToHashSet(StringComparer.OrdinalIgnoreCase).SetEquals(["below60", "60to79", "80plus"]) || regime.AgeCategories.Values.Any(slabs => slabs.Count == 0 || slabs[^1].UpTo != 0 || slabs.Any(s => s.Rate < 0 || s.UpTo < 0))) throw new InvalidDataException($"Tax rules file '{path}' has invalid slabs for regime '{name}'.");
        _cache[year] = rules;
        return rules;
    }
}
