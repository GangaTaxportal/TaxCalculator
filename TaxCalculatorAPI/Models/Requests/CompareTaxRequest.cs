using System.ComponentModel.DataAnnotations;

namespace TaxCalculatorAPI.Models.Requests;

public sealed class CompareTaxRequest
{
    [Required] public string TaxYear { get; set; } = string.Empty;
    [Required] public string AgeCategory { get; set; } = string.Empty;
    [Range(typeof(decimal), "0.01", "79228162514264337593543950335")] public decimal GrossSalary { get; set; }
    [Range(typeof(decimal), "0", "79228162514264337593543950335")] public decimal SalaryAllowancesExemptBothRegimes { get; set; }
    [Range(typeof(decimal), "0", "79228162514264337593543950335")] public decimal OtherIncome { get; set; }
    [Range(typeof(decimal), "0", "79228162514264337593543950335")] public decimal HousePropertyInterest { get; set; }
    [Required] public OldRegimeDeductionsRequest OldRegimeDeductions { get; set; } = new();
    [Range(typeof(decimal), "0", "79228162514264337593543950335")] public decimal NewRegimeDeductions { get; set; }
}

public sealed class OldRegimeDeductionsRequest
{
    [Range(typeof(decimal), "0", "79228162514264337593543950335")] public decimal Section80C { get; set; }
    [Range(typeof(decimal), "0", "79228162514264337593543950335")] public decimal Section80D { get; set; }
    [Range(typeof(decimal), "0", "79228162514264337593543950335")] public decimal OtherDeductions { get; set; }
}
