using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Common.Responses;
using app.Data.EF;
using app.DTOs.Payments;
using app.Repositories.Payments;

namespace app.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentRepository _repo;
    private readonly AppDbContext _db;
    public PaymentsController(IPaymentRepository repo, AppDbContext db) { _repo = repo; _db = db; }

    [HttpGet]
    public async Task<IResult> List([FromQuery] PaymentListQuery query, CancellationToken cancellationToken)
    {
        await EnsureCanViewAsync(cancellationToken);
        var result = await _repo.GetAllAsync(query, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Get payments successfully."));
    }

    [HttpGet("{id:long}")]
    public async Task<IResult> Detail(long id, CancellationToken cancellationToken)
    {
        await EnsureCanViewAsync(cancellationToken);
        var result = await _repo.GetByIdAsync(id, cancellationToken);
        if (result is null) throw new KeyNotFoundException("Payment not found.");
        return Results.Ok(ApiResponse.Ok(result, "Get payment successfully."));
    }

    [HttpPost]
    public async Task<IResult> Create([FromBody] CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        var staffId = await EnsureSalesAsync(cancellationToken);
        var result = await _repo.CreateAsync(request, staffId, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Create payment request successfully."));
    }

    [HttpPost("{id:long}/confirm")]
    public async Task<IResult> Confirm(long id, CancellationToken cancellationToken)
    {
        await EnsureOperatorOrAdminAsync(cancellationToken);
        var result = await _repo.ConfirmAsync(id, cancellationToken);
        if (result is null) throw new KeyNotFoundException("Payment not found.");
        return Results.Ok(ApiResponse.Ok(result, "Confirm payment successfully."));
    }

    [HttpPost("{id:long}/refund")]
    public async Task<IResult> Refund(long id, CancellationToken cancellationToken)
    {
        await EnsureOperatorOrAdminAsync(cancellationToken);
        var result = await _repo.RefundAsync(id, cancellationToken);
        if (result is null) throw new KeyNotFoundException("Payment not found.");
        return Results.Ok(ApiResponse.Ok(result, "Refund payment successfully."));
    }

    private async Task EnsureCanViewAsync(CancellationToken cancellationToken)
    {
        var role = User.FindFirst("role")?.Value;
        if (role == "1") return;
        if (role != "2") throw new UnauthorizedAccessException("Only staff/admin can view payments.");
        var dep = await GetDepartmentAsync(cancellationToken);
        if (dep != 1 && dep != 2 && dep != 3) throw new UnauthorizedAccessException("Only sales/academic/operation can view payments.");
    }

    private async Task<long> EnsureSalesAsync(CancellationToken cancellationToken)
    {
        var role = User.FindFirst("role")?.Value;
        if (role != "2") throw new UnauthorizedAccessException("Only sales can create payment request.");
        var userId = GetCurrentUserId();
        var staff = await _db.Staff.AsNoTracking().Where(x => x.UserId == userId).Select(x => new { x.Id, x.Department }).FirstOrDefaultAsync(cancellationToken);
        if (staff is null || staff.Department != 1) throw new UnauthorizedAccessException("Only sales can create payment request.");
        return staff.Id;
    }

    private async Task EnsureOperatorOrAdminAsync(CancellationToken cancellationToken)
    {
        var role = User.FindFirst("role")?.Value;
        if (role == "1") return;
        if (role != "2") throw new UnauthorizedAccessException("Only operator/admin can confirm or refund payment.");
        var dep = await GetDepartmentAsync(cancellationToken);
        if (dep != 3) throw new UnauthorizedAccessException("Only operator/admin can confirm or refund payment.");
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
