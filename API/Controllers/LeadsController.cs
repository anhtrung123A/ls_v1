using app.Application.DTOs;
using app.Application.DTOs.Responses;
using app.Application.UseCases;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace app.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Authorize(Policy = "LeadCrud")]
[Route("api/v{version:apiVersion}/leads")]
public class LeadsController : ControllerBase
{
    private readonly CreateLeadUseCase _createLeadUseCase;
    private readonly GetLeadsUseCase _getLeadsUseCase;
    private readonly GetLeadByIdUseCase _getLeadByIdUseCase;
    private readonly UpdateLeadUseCase _updateLeadUseCase;
    private readonly DeleteLeadUseCase _deleteLeadUseCase;
    private readonly GetAssignedLeadsByUserUseCase _getAssignedLeadsByUserUseCase;

    public LeadsController(
        CreateLeadUseCase createLeadUseCase,
        GetLeadsUseCase getLeadsUseCase,
        GetLeadByIdUseCase getLeadByIdUseCase,
        UpdateLeadUseCase updateLeadUseCase,
        DeleteLeadUseCase deleteLeadUseCase,
        GetAssignedLeadsByUserUseCase getAssignedLeadsByUserUseCase)
    {
        _createLeadUseCase = createLeadUseCase;
        _getLeadsUseCase = getLeadsUseCase;
        _getLeadByIdUseCase = getLeadByIdUseCase;
        _updateLeadUseCase = updateLeadUseCase;
        _deleteLeadUseCase = deleteLeadUseCase;
        _getAssignedLeadsByUserUseCase = getAssignedLeadsByUserUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLeadDto dto, CancellationToken cancellationToken)
    {
        var lead = await _createLeadUseCase.ExecuteAsync(User, dto, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<LeadDto>.Ok(lead, "Lead created successfully."));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PaginationQueryDto pagination, CancellationToken cancellationToken)
    {
        var leads = await _getLeadsUseCase.ExecuteAsync(pagination, cancellationToken);
        return Ok(ApiResponse<PaginatedResultDto<LeadDto>>.Ok(leads, "Leads fetched successfully."));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(ulong id, CancellationToken cancellationToken)
    {
        var lead = await _getLeadByIdUseCase.ExecuteAsync(id, cancellationToken);
        return Ok(ApiResponse<LeadDto>.Ok(lead, "Lead fetched successfully."));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(ulong id, [FromBody] UpdateLeadDto dto, CancellationToken cancellationToken)
    {
        var lead = await _updateLeadUseCase.ExecuteAsync(User, id, dto, cancellationToken);
        return Ok(ApiResponse<LeadDto>.Ok(lead, "Lead updated successfully."));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(ulong id, CancellationToken cancellationToken)
    {
        await _deleteLeadUseCase.ExecuteAsync(User, id, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(true, "Lead deleted successfully."));
    }

    [HttpGet("assigned/{userId:long}")]
    public async Task<IActionResult> GetAssignedLeads(ulong userId, [FromQuery] PaginationQueryDto pagination, CancellationToken cancellationToken)
    {
        var leads = await _getAssignedLeadsByUserUseCase.ExecuteAsync(userId, pagination, cancellationToken);
        return Ok(ApiResponse<PaginatedResultDto<LeadDto>>.Ok(leads, "Assigned leads fetched successfully."));
    }
}
