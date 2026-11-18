using Microsoft.AspNetCore.Mvc;
using TaxCalculatorAPI.Models.Requests;
using TaxCalculatorAPI.Services.Interfaces;
using TaxCalculatorAPI.Validation;
namespace TaxCalculatorAPI.Controllers;
[ApiController]
[Route("api/tax")]
public sealed class TaxController(ITaxComparisonService service) : ControllerBase
{
    [HttpPost("compare")]
    public IActionResult Compare([FromBody] CompareTaxRequest request)
    {
        var result = service.Compare(request);
        return Ok(result);

        //return result.ResultStatus switch
        //{
        //    Models.ResultStatus.Success => Ok(result.Data),
        //    Models.ResultStatus.ValidationFailure => BadRequest(new ValidationProblemDetails(
        //        new Dictionary<string, string[]> { ["request"] = [result.Message] })),
        //    _ => Problem(statusCode: StatusCodes.Status500InternalServerError, title: "Tax calculation failed", detail: result.Message)
        //};
    }
}
