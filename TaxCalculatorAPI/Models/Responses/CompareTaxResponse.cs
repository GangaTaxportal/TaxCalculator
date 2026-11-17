namespace TaxCalculatorAPI.Models.Responses;
public sealed class CompareTaxResponse
{
    public string TaxYear { get; set; } = "";
    public string RecommendedRegime { get; set; } = "";
    public decimal Savings { get; set; }
    public RegimeResultResponse OldRegime { get; set; } = new();
    public RegimeResultResponse NewRegime { get; set; } = new();
}
