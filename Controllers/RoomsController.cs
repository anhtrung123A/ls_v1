using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Common.Responses;
using app.Data.EF;
using app.DTOs.Rooms;
using app.Repositories.Rooms;

namespace app.Controllers;

[ApiController]
[Route("api/rooms")]
[Authorize]
public class RoomsController : ControllerBase
{
    private readonly IRoomRepository _repo;
    private readonly AppDbContext _db;

    public RoomsController(IRoomRepository repo, AppDbContext db) { _repo = repo; _db = db; }

    [HttpGet]
    public async Task<IResult> List([FromQuery] RoomListQuery query, CancellationToken cancellationToken)
    {
        await EnsureRoomPermissionAsync(cancellationToken);
        var result = await _repo.GetAllAsync(query, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Get rooms successfully."));
    }

    [HttpGet("{id:long}")]
    public async Task<IResult> Detail(long id, CancellationToken cancellationToken)
    {
        await EnsureRoomPermissionAsync(cancellationToken);
        var result = await _repo.GetByIdAsync(id, cancellationToken);
        if (result is null) throw new KeyNotFoundException("Room not found.");
        return Results.Ok(ApiResponse.Ok(result, "Get room successfully."));
    }

    [HttpPost]
    public async Task<IResult> Create([FromBody] CreateRoomRequest request, CancellationToken cancellationToken)
    {
        await EnsureRoomPermissionAsync(cancellationToken);
        var result = await _repo.CreateAsync(new RoomRequest
        {
            Name = request.Name,
            Location = request.Location,
            Capacity = request.Capacity,
            Facilities = request.Facilities
        }, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Create room successfully."));
    }

    [HttpPut("{id:long}")]
    public async Task<IResult> Update(long id, [FromBody] RoomRequest request, CancellationToken cancellationToken)
    {
        await EnsureRoomPermissionAsync(cancellationToken);
        var result = await _repo.UpdateAsync(id, request, cancellationToken);
        if (result is null) throw new KeyNotFoundException("Room not found.");
        return Results.Ok(ApiResponse.Ok(result, "Update room successfully."));
    }

    [HttpDelete("{id:long}")]
    public async Task<IResult> Delete(long id, CancellationToken cancellationToken)
    {
        await EnsureRoomPermissionAsync(cancellationToken);
        await _repo.DeleteAsync(id, cancellationToken);
        return Results.Ok(ApiResponse.Ok(null, "Delete room successfully."));
    }

    private async Task EnsureRoomPermissionAsync(CancellationToken cancellationToken)
    {
        var role = User.FindFirst("role")?.Value;
        if (role == "1") return;
        if (role != "2") throw new UnauthorizedAccessException("Only admin or operation can manage rooms.");

        var dep = await GetDepartmentAsync(cancellationToken);
        if (dep != 3) throw new UnauthorizedAccessException("Only admin or operation can manage rooms.");
    }

    private async Task<byte?> GetDepartmentAsync(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        return await _db.Staff.AsNoTracking().Where(x => x.UserId == userId).Select(x => x.Department).FirstOrDefaultAsync(cancellationToken);
    }

    private long GetCurrentUserId()
    {
        var subject = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!long.TryParse(subject, out var userId)) throw new UnauthorizedAccessException("Invalid access token.");
        return userId;
    }
}
