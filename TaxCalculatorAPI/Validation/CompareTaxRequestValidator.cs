using TaxCalculatorAPI.Models.Requests;
namespace TaxCalculatorAPI.Validation;
public sealed class CompareTaxRequestValidator
{
    private static readonly string[] Ages = ["below60", "60to79", "80plus"];
    public Dictionary<string, string[]> Validate(CompareTaxRequest request)
    {
        var errors = new Dictionary<string, string[]>();
        void Add(string key, string message) => errors[key] = [message];
        if (request.GrossSalary <= 0) Add(nameof(request.GrossSalary), "Gross salary must be greater than zero.");
        if (request.SalaryAllowancesExemptBothRegimes < 0 || request.OtherIncome < 0 || request.HousePropertyInterest < 0 || request.NewRegimeDeductions < 0 || request.OldRegimeDeductions is null || request.OldRegimeDeductions.Section80C < 0 || request.OldRegimeDeductions.Section80D < 0 || request.OldRegimeDeductions.OtherDeductions < 0)
            Add("amounts", "All monetary values must be non-negative.");
        if (request.OldRegimeDeductions is null) Add(nameof(request.OldRegimeDeductions), "Old regime deductions are required.");
        if (!Ages.Contains(request.AgeCategory, StringComparer.OrdinalIgnoreCase)) Add(nameof(request.AgeCategory), "Age category must be below60, 60to79, or 80plus.");
        if (string.IsNullOrWhiteSpace(request.TaxYear)) Add(nameof(request.TaxYear), "Tax year is required.");
        return errors;
    }
}
