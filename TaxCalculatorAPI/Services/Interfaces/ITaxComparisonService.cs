using TaxCalculatorAPI.Models.Requests;
using TaxCalculatorAPI.Models.Responses;
using TaxCalculatorAPI.Models;
namespace TaxCalculatorAPI.Services.Interfaces;
public interface ITaxComparisonService { OperationResult<CompareTaxResponse> Compare(CompareTaxRequest request); }
