using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Common.Responses;
using app.Data.EF;
using app.DTOs.Interactions;
using app.Repositories.Interactions;

namespace app.Controllers;

[ApiController]
[Route("api/interactions")]
[Authorize]
public class InteractionsController : ControllerBase
{
    private readonly IInteractionRepository _interactionRepository;
    private readonly AppDbContext _dbContext;

    public InteractionsController(IInteractionRepository interactionRepository, AppDbContext dbContext)
    {
        _interactionRepository = interactionRepository;
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<IResult> List([FromQuery] InteractionListQuery query, CancellationToken cancellationToken)
    {
        var context = await GetAccessContextAsync(cancellationToken);

        if (!context.CanManage)
        {
            query.StaffId = context.StaffId;
        }

        var items = await _interactionRepository.GetAllAsync(query, cancellationToken);
        return Results.Ok(ApiResponse.Ok(items, "Get interactions successfully."));
    }

    [HttpGet("lead/{lead_id:long}")]
    public async Task<IResult> ListByLeadId(long lead_id, [FromQuery] InteractionListQuery query, CancellationToken cancellationToken)
    {
        var context = await GetAccessContextAsync(cancellationToken);

        if (!context.CanManage)
        {
            query.StaffId = context.StaffId;
        }

        var items = await _interactionRepository.GetByLeadIdAsync(lead_id, query, cancellationToken);
        return Results.Ok(ApiResponse.Ok(items, "Get interactions by lead successfully."));
    }

    [HttpGet("{id:long}")]
    public async Task<IResult> Detail(long id, CancellationToken cancellationToken)
    {
        var context = await GetAccessContextAsync(cancellationToken);
        var result = await _interactionRepository.GetByIdAsync(id, cancellationToken);
        if (result is null)
        {
            throw new KeyNotFoundException("Interaction not found.");
        }

        if (!context.CanManage && result.StaffId != context.StaffId)
        {
            throw new UnauthorizedAccessException("You can only view your own interactions.");
        }

        return Results.Ok(ApiResponse.Ok(result, "Get interaction successfully."));
    }

    [HttpPost]
    public async Task<IResult> Create([FromBody] InteractionRequest request, CancellationToken cancellationToken)
    {
        var context = await GetAccessContextAsync(cancellationToken);

        if (!context.CanManage)
        {
            request.StaffId = context.StaffId;
        }

        var result = await _interactionRepository.CreateAsync(request, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Create interaction successfully."));
    }

    [HttpPut("{id:long}")]
    public async Task<IResult> Update(long id, [FromBody] InteractionRequest request, CancellationToken cancellationToken)
    {
        var context = await GetAccessContextAsync(cancellationToken);

        if (!context.CanManage)
        {
            var existed = await _interactionRepository.GetByIdAsync(id, cancellationToken);
            if (existed is null)
            {
                throw new KeyNotFoundException("Interaction not found.");
            }

            if (existed.StaffId != context.StaffId)
            {
                throw new UnauthorizedAccessException("You can only update your own interactions.");
            }

            request.StaffId = context.StaffId;
        }

        var result = await _interactionRepository.UpdateAsync(id, request, cancellationToken);
        if (result is null)
        {
            throw new KeyNotFoundException("Interaction not found.");
        }

        return Results.Ok(ApiResponse.Ok(result, "Update interaction successfully."));
    }

    [HttpDelete("{id:long}")]
    public async Task<IResult> Delete(long id, CancellationToken cancellationToken)
    {
        var context = await GetAccessContextAsync(cancellationToken);

        if (!context.CanManage)
        {
            var existed = await _interactionRepository.GetByIdAsync(id, cancellationToken);
            if (existed is null)
            {
                throw new KeyNotFoundException("Interaction not found.");
            }

            if (existed.StaffId != context.StaffId)
            {
                throw new UnauthorizedAccessException("You can only delete your own interactions.");
            }
        }

        await _interactionRepository.DeleteAsync(id, cancellationToken);
        return Results.Ok(ApiResponse.Ok(null, "Delete interaction successfully."));
    }

    private async Task<(bool CanManage, long StaffId)> GetAccessContextAsync(CancellationToken cancellationToken)
    {
        var role = User.FindFirst("role")?.Value;
        if (role == "1")
        {
            return (true, 0);
        }

        if (role != "2")
        {
            throw new UnauthorizedAccessException("Only staff can manage interactions.");
        }

        var userId = GetCurrentUserId();
        var staff = await _dbContext.Staff
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => new { x.Id, x.Department })
            .FirstOrDefaultAsync(cancellationToken);

        if (staff is null)
        {
            throw new UnauthorizedAccessException("Staff profile not found.");
        }

        if (staff.Department == 4)
        {
            return (true, staff.Id);
        }

        if (staff.Department == 1 || staff.Department == 3)
        {
            return (false, staff.Id);
        }

        throw new UnauthorizedAccessException("Only sales/cskh/management can manage interactions.");
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
