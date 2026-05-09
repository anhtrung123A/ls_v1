using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using app.Common.Responses;
using app.Data.EF;
using app.DTOs.Invoices;
using app.Repositories.Invoices;

namespace app.Controllers;

[ApiController]
[Route("api/invoices")]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceRepository _repo;
    private readonly AppDbContext _db;
    public InvoicesController(IInvoiceRepository repo, AppDbContext db) { _repo = repo; _db = db; }

    [HttpGet]
    public async Task<IResult> List([FromQuery] InvoiceListQuery query, CancellationToken cancellationToken)
    {
        await EnsureCanViewAsync(cancellationToken);
        var result = await _repo.GetAllAsync(query, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Get invoices successfully."));
    }

    [HttpGet("{id:long}")]
    public async Task<IResult> Detail(long id, CancellationToken cancellationToken)
    {
        await EnsureCanViewAsync(cancellationToken);
        var result = await _repo.GetByIdAsync(id, cancellationToken);
        if (result is null) throw new KeyNotFoundException("Invoice not found.");
        return Results.Ok(ApiResponse.Ok(result, "Get invoice successfully."));
    }

    [HttpPost]
    public async Task<IResult> Create([FromBody] CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        await EnsureSalesAsync(cancellationToken);
        var result = await _repo.CreateAsync(request, GetCurrentUserId(), cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Create invoice successfully."));
    }

    private async Task EnsureCanViewAsync(CancellationToken cancellationToken)
    {
        var role = User.FindFirst("role")?.Value;
        if (role == "1") return;
        if (role != "2") throw new UnauthorizedAccessException("Only staff/admin can view invoices.");
        var dep = await GetDepartmentAsync(cancellationToken);
        if (dep != 1 && dep != 2 && dep != 3) throw new UnauthorizedAccessException("Only sales/academic/operation can view invoices.");
    }

    private async Task EnsureSalesAsync(CancellationToken cancellationToken)
    {
        var role = User.FindFirst("role")?.Value;
        if (role != "2") throw new UnauthorizedAccessException("Only sales can create invoice.");
        var dep = await GetDepartmentAsync(cancellationToken);
        if (dep != 1) throw new UnauthorizedAccessException("Only sales can create invoice.");
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
