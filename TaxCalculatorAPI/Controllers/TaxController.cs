using Microsoft.AspNetCore.Mvc;
using TaxCalculatorAPI.Models.Requests;
using TaxCalculatorAPI.Services.Interfaces;
using TaxCalculatorAPI.Validation;
namespace TaxCalculatorAPI.Controllers;
[ApiController]
[Route("api/tax")]
public sealed class TaxController(ITaxComparisonService service, CompareTaxRequestValidator validator) : ControllerBase
{
    [HttpPost("compare")]
    public IActionResult Compare([FromBody] CompareTaxRequest request)
    {
        var errors = validator.Validate(request);
        if (errors.Count > 0) return BadRequest(new ValidationProblemDetails(errors));
        try { return Ok(service.Compare(request)); }
        catch (KeyNotFoundException ex) { return BadRequest(new ProblemDetails { Status = 400, Title = "Unsupported tax year", Detail = ex.Message }); }
    }
}
