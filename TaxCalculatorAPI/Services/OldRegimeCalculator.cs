using TaxCalculatorAPI.Models;
using TaxCalculatorAPI.Models.Requests;
using TaxCalculatorAPI.Models.Responses;
namespace TaxCalculatorAPI.Services;
public sealed class OldRegimeCalculator(SlabCalculator slabs)
{
    public RegimeResultResponse Calculate(CompareTaxRequest request, TaxRules rules)
    {
        var r = rules.Regimes["old"]; var gross = Math.Max(0, request.GrossSalary - request.SalaryAllowancesExemptBothRegimes + request.OtherIncome - request.HousePropertyInterest);
        var c = Math.Min(request.OldRegimeDeductions.Section80C, r.Section80CLimit); var d = Math.Min(request.OldRegimeDeductions.Section80D, r.Section80DLimit); var o = Math.Min(request.OldRegimeDeductions.OtherDeductions, r.OtherDeductionsLimit);
        var deductions = c + d + o; var taxable = Math.Max(0, gross - r.StandardDeduction - deductions); var slabTax = slabs.Calculate(taxable, r.AgeCategories[request.AgeCategory.ToLowerInvariant()]);
        return TaxResultFactory.Create(gross, r.StandardDeduction, deductions, taxable, slabTax, r, rules.CessRate, []);
    }
}
