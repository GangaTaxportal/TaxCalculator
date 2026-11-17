using TaxCalculatorAPI.Models.Requests;
using TaxCalculatorAPI.Models.Responses;
namespace TaxCalculatorAPI.Services.Interfaces;
public interface ITaxComparisonService { CompareTaxResponse Compare(CompareTaxRequest request); }
