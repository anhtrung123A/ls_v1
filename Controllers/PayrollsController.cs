using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using app.Common.Responses;
using app.DTOs.Payrolls;
using app.Repositories.Payrolls;

namespace app.Controllers;

[ApiController]
[Route("api/payrolls")]
[Authorize]
public class PayrollsController : ControllerBase
{
    private readonly IPayrollRepository _repo;
    public PayrollsController(IPayrollRepository repo) { _repo = repo; }

    [HttpGet]
    public async Task<IResult> List([FromQuery] PayrollListQuery query, CancellationToken cancellationToken)
    {
        EnsureAdminPermission();
        var result = await _repo.GetAllAsync(query, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Get payrolls successfully."));
    }

    [HttpGet("{id:long}")]
    public async Task<IResult> Detail(long id, CancellationToken cancellationToken)
    {
        EnsureAdminPermission();
        var result = await _repo.GetByIdAsync(id, cancellationToken);
        if (result is null) throw new KeyNotFoundException("Payroll not found.");
        return Results.Ok(ApiResponse.Ok(result, "Get payroll detail successfully."));
    }

    [HttpPut("{id:long}/status")]
    public async Task<IResult> UpdateStatus(long id, [FromBody] UpdatePayrollStatusRequest request, CancellationToken cancellationToken)
    {
        EnsureAdminPermission();
        var result = await _repo.UpdateStatusAsync(id, request, cancellationToken);
        if (result is null) throw new KeyNotFoundException("Payroll not found.");
        return Results.Ok(ApiResponse.Ok(result, "Update payroll status successfully."));
    }

    private void EnsureAdminPermission()
    {
        var role = User.FindFirst("role")?.Value;
        if (role != "1") throw new UnauthorizedAccessException("Only admin can manage payrolls.");
    }
}
