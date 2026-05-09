using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using app.Common.Responses;
using app.DTOs.SalaryConfigs;
using app.Repositories.SalaryConfigs;

namespace app.Controllers;

[ApiController]
[Route("api/salary_configs")]
[Authorize]
public class SalaryConfigsController : ControllerBase
{
    private readonly ISalaryConfigRepository _repo;
    public SalaryConfigsController(ISalaryConfigRepository repo) { _repo = repo; }

    [HttpGet]
    public async Task<IResult> List([FromQuery] SalaryConfigListQuery query, CancellationToken cancellationToken)
    {
        EnsureAdminPermission();
        var result = await _repo.GetAllAsync(query, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Get salary configs successfully."));
    }

    [HttpGet("{id:long}")]
    public async Task<IResult> Detail(long id, CancellationToken cancellationToken)
    {
        EnsureAdminPermission();
        var result = await _repo.GetByIdAsync(id, cancellationToken);
        if (result is null) throw new KeyNotFoundException("Salary config not found.");
        return Results.Ok(ApiResponse.Ok(result, "Get salary config successfully."));
    }

    [HttpPost]
    public async Task<IResult> Create([FromBody] SalaryConfigRequest request, CancellationToken cancellationToken)
    {
        EnsureAdminPermission();
        var result = await _repo.CreateAsync(request, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Create salary config successfully."));
    }

    [HttpPut("{id:long}")]
    public async Task<IResult> Update(long id, [FromBody] SalaryConfigRequest request, CancellationToken cancellationToken)
    {
        EnsureAdminPermission();
        var result = await _repo.UpdateAsync(id, request, cancellationToken);
        if (result is null) throw new KeyNotFoundException("Salary config not found.");
        return Results.Ok(ApiResponse.Ok(result, "Update salary config successfully."));
    }

    [HttpDelete("{id:long}")]
    public async Task<IResult> Delete(long id, CancellationToken cancellationToken)
    {
        EnsureAdminPermission();
        await _repo.DeleteAsync(id, cancellationToken);
        return Results.Ok(ApiResponse.Ok(null, "Delete salary config successfully."));
    }

    private void EnsureAdminPermission()
    {
        var role = User.FindFirst("role")?.Value;
        if (role != "1") throw new UnauthorizedAccessException("Only admin can manage salary configs.");
    }
}
