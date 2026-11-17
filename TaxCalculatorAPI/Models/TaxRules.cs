namespace TaxCalculatorAPI.Models;
public sealed class TaxRules
{
    public string TaxYear { get; set; } = "";
    public string VerifiedAgainst { get; set; } = "";
    public decimal CessRate { get; set; }
    public Dictionary<string, RegimeRules> Regimes { get; set; } = [];
}
public sealed class RegimeRules
{
    public decimal StandardDeduction { get; set; }
    public decimal RebateThreshold { get; set; }
    public decimal MaximumRebate { get; set; }
    public bool AllowsDeductions { get; set; }
    public bool AllowsNewRegimeDeductions { get; set; }
    public decimal Section80CLimit { get; set; }
    public decimal Section80DLimit { get; set; }
    public decimal OtherDeductionsLimit { get; set; }
    public Dictionary<string, List<TaxSlab>> AgeCategories { get; set; } = [];
}
public sealed class TaxSlab
{
    public decimal UpTo { get; set; }
    public decimal Rate { get; set; }
}
