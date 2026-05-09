using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using app.Common.Responses;
using app.DTOs.Staff;
using app.Repositories.Staff;

namespace app.Controllers;

[ApiController]
[Route("api/staff")]
[Authorize]
public class StaffController : ControllerBase
{
    private readonly IStaffRepository _staffRepository;

    public StaffController(IStaffRepository staffRepository)
    {
        _staffRepository = staffRepository;
    }

    [HttpPost]
    public async Task<IResult> Create([FromBody] CreateStaffRequest request, CancellationToken cancellationToken)
    {
        EnsureAdminRole();
        var result = await _staffRepository.CreateAsync(request, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Create staff successfully."));
    }

    [HttpDelete("{id:long}")]
    public async Task<IResult> Delete(long id, CancellationToken cancellationToken)
    {
        EnsureAdminRole();
        await _staffRepository.DeleteAsync(id, cancellationToken);
        return Results.Ok(ApiResponse.Ok(null, "Delete staff successfully."));
    }

    private void EnsureAdminRole()
    {
        var role = User.FindFirst("role")?.Value;
        if (role != "1")
        {
            throw new UnauthorizedAccessException("Only admin can perform this action.");
        }
    }
}
