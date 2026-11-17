namespace TaxCalculatorAPI.Models.Responses;
public sealed class RegimeResultResponse
{
    public decimal GrossTotalIncome { get; set; }
    public decimal StandardDeduction { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal TaxableIncome { get; set; }
    public decimal SlabTax { get; set; }
    public decimal Rebate87A { get; set; }
    public decimal TaxAfterRebate { get; set; }
    public decimal Surcharge { get; set; }
    public decimal HealthAndEducationCess { get; set; }
    public decimal FinalTax { get; set; }
    public List<string> Warnings { get; set; } = [];
}
