using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Common.Responses;
using app.Data.EF;
using app.DTOs.Leads;
using app.Repositories.Leads;

namespace app.Controllers;

[ApiController]
[Route("api/leads")]
[Authorize]
public class LeadController : ControllerBase
{
    private readonly ILeadRepository _leadRepository;
    private readonly AppDbContext _dbContext;

    public LeadController(ILeadRepository leadRepository, AppDbContext dbContext)
    {
        _leadRepository = leadRepository;
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IResult> GetAll([FromQuery] LeadListQuery query, CancellationToken cancellationToken)
    {
        var context = await GetAccessContextAsync(cancellationToken);

        if (!context.CanManage && !context.CanViewOwnAssigned)
        {
            throw new UnauthorizedAccessException("You do not have permission to view leads.");
        }

        if (context.CanViewOwnAssigned)
        {
            if (!context.StaffId.HasValue)
            {
                throw new UnauthorizedAccessException("Staff profile is required.");
            }

            query.AssignedTo = context.StaffId.Value;
        }

        var result = await _leadRepository.GetAllAsync(query, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Get leads successfully."));
    }

    [HttpGet("assigned_lead")]
    public async Task<IResult> GetAssignedLead([FromQuery] LeadListQuery query, CancellationToken cancellationToken)
    {
        var context = await GetAccessContextAsync(cancellationToken);
        if (!context.CanViewOwnAssigned || !context.StaffId.HasValue)
        {
            throw new UnauthorizedAccessException("Only sales staff can access assigned leads.");
        }

        query.AssignedTo = context.StaffId.Value;
        var result = await _leadRepository.GetAllAsync(query, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Get assigned leads successfully."));
    }

    [HttpGet("{id:long}")]
    public async Task<IResult> GetById(long id, CancellationToken cancellationToken)
    {
        var context = await GetAccessContextAsync(cancellationToken);
        var result = await _leadRepository.GetByIdAsync(id, cancellationToken);
        if (result is null)
        {
            throw new KeyNotFoundException("Lead not found.");
        }

        if (context.CanViewOwnAssigned && result.AssignedTo != context.StaffId)
        {
            throw new UnauthorizedAccessException("You can only view leads assigned to you.");
        }

        if (!context.CanManage && !context.CanViewOwnAssigned)
        {
            throw new UnauthorizedAccessException("You do not have permission to view this lead.");
        }

        return Results.Ok(ApiResponse.Ok(result, "Get lead successfully."));
    }

    [HttpPost]
    public async Task<IResult> Create([FromBody] CreateLeadRequest request, CancellationToken cancellationToken)
    {
        await EnsureLeadManagePermissionAsync(cancellationToken);
        var result = await _leadRepository.CreateAsync(request, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Create lead successfully."));
    }

    [HttpPut("{id:long}")]
    public async Task<IResult> Update(long id, [FromBody] UpdateLeadRequest request, CancellationToken cancellationToken)
    {
        await EnsureLeadManagePermissionAsync(cancellationToken);
        var result = await _leadRepository.UpdateAsync(id, request, cancellationToken);
        if (result is null)
        {
            throw new KeyNotFoundException("Lead not found.");
        }

        return Results.Ok(ApiResponse.Ok(result, "Update lead successfully."));
    }

    [HttpPut("{id:long}/assign")]
    public async Task<IResult> AssignLead(long id, [FromBody] AssignLeadRequest request, CancellationToken cancellationToken)
    {
        await EnsureLeadManagePermissionAsync(cancellationToken);
        var result = await _leadRepository.AssignAsync(id, request.AssignedTo, cancellationToken);
        if (result is null)
        {
            throw new KeyNotFoundException("Lead not found.");
        }

        return Results.Ok(ApiResponse.Ok(result, "Assign lead successfully."));
    }

    [HttpPost("{id:long}/convert")]
    public async Task<IResult> ConvertLead(long id, CancellationToken cancellationToken)
    {
        var context = await GetAccessContextAsync(cancellationToken);
        if (!context.StaffId.HasValue)
        {
            throw new UnauthorizedAccessException("Staff profile is required.");
        }

        var lead = await _leadRepository.GetByIdAsync(id, cancellationToken);
        if (lead is null)
        {
            throw new KeyNotFoundException("Lead not found.");
        }

        if (lead.AssignedTo != context.StaffId.Value)
        {
            throw new UnauthorizedAccessException("Only assigned staff can convert this lead.");
        }

        var result = await _leadRepository.ConvertToStudentAsync(id, cancellationToken);
        if (result is null)
        {
            throw new KeyNotFoundException("Lead not found.");
        }

        return Results.Ok(ApiResponse.Ok(result, "Convert lead successfully."));
    }

    [HttpDelete("{id:long}")]
    public async Task<IResult> Delete(long id, CancellationToken cancellationToken)
    {
        await EnsureLeadManagePermissionAsync(cancellationToken);
        await _leadRepository.DeleteAsync(id, cancellationToken);
        return Results.Ok(ApiResponse.Ok(null, "Delete lead successfully."));
    }

    private async Task EnsureLeadManagePermissionAsync(CancellationToken cancellationToken)
    {
        var context = await GetAccessContextAsync(cancellationToken);
        if (!context.CanManage)
        {
            throw new UnauthorizedAccessException("Only admin or management staff can manage leads.");
        }
    }

    private async Task<(bool CanManage, bool CanViewOwnAssigned, long? StaffId)> GetAccessContextAsync(CancellationToken cancellationToken)
    {
        var role = User.FindFirst("role")?.Value;
        if (role == "1")
        {
            return (true, false, null);
        }

        if (role != "2")
        {
            return (false, false, null);
        }

        var staffIdClaim = User.FindFirst("staff_id")?.Value;
        var departmentClaim = User.FindFirst("department")?.Value;

        if (long.TryParse(staffIdClaim, out var claimStaffId) && byte.TryParse(departmentClaim, out var claimDepartment))
        {
            return claimDepartment switch
            {
                4 => (true, false, claimStaffId),
                1 => (false, true, claimStaffId),
                _ => (false, false, claimStaffId)
            };
        }

        var userId = GetCurrentUserId();
        var staff = await _dbContext.Staff
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .Select(s => new { s.Id, s.Department })
            .FirstOrDefaultAsync(cancellationToken);

        if (staff is null)
        {
            return (false, false, null);
        }

        return staff.Department switch
        {
            4 => (true, false, staff.Id),
            1 => (false, true, staff.Id),
            _ => (false, false, staff.Id)
        };
    }

    private long GetCurrentUserId()
    {
        var subject = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!long.TryParse(subject, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid access token.");
        }

        return userId;
    }
}
