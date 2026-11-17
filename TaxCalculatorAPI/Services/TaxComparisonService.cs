using TaxCalculatorAPI.Models.Requests;
using TaxCalculatorAPI.Models.Responses;
using TaxCalculatorAPI.Services.Interfaces;
namespace TaxCalculatorAPI.Services;
public sealed class TaxComparisonService(TaxRuleProvider rules, OldRegimeCalculator oldCalculator, NewRegimeCalculator newCalculator) : ITaxComparisonService
{
    public CompareTaxResponse Compare(CompareTaxRequest request)
    {
        var data = rules.Get(request.TaxYear); var old = oldCalculator.Calculate(request, data); var newer = newCalculator.Calculate(request, data);
        return new() { TaxYear = data.TaxYear, OldRegime = old, NewRegime = newer, RecommendedRegime = old.FinalTax < newer.FinalTax ? "Old" : newer.FinalTax < old.FinalTax ? "New" : "Same", Savings = Math.Abs(old.FinalTax - newer.FinalTax) };
    }
}
