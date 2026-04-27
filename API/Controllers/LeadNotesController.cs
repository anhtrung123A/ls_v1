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
[Route("api/v{version:apiVersion}/leads/{leadId:long}/notes")]
public class LeadNotesController : ControllerBase
{
    private readonly CreateLeadNoteUseCase _createLeadNoteUseCase;
    private readonly GetLeadNotesUseCase _getLeadNotesUseCase;
    private readonly GetLeadNoteByIdUseCase _getLeadNoteByIdUseCase;
    private readonly UpdateLeadNoteUseCase _updateLeadNoteUseCase;
    private readonly DeleteLeadNoteUseCase _deleteLeadNoteUseCase;

    public LeadNotesController(
        CreateLeadNoteUseCase createLeadNoteUseCase,
        GetLeadNotesUseCase getLeadNotesUseCase,
        GetLeadNoteByIdUseCase getLeadNoteByIdUseCase,
        UpdateLeadNoteUseCase updateLeadNoteUseCase,
        DeleteLeadNoteUseCase deleteLeadNoteUseCase)
    {
        _createLeadNoteUseCase = createLeadNoteUseCase;
        _getLeadNotesUseCase = getLeadNotesUseCase;
        _getLeadNoteByIdUseCase = getLeadNoteByIdUseCase;
        _updateLeadNoteUseCase = updateLeadNoteUseCase;
        _deleteLeadNoteUseCase = deleteLeadNoteUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Create(ulong leadId, [FromBody] CreateLeadNoteDto dto, CancellationToken cancellationToken)
    {
        var leadNote = await _createLeadNoteUseCase.ExecuteAsync(User, leadId, dto, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<LeadNoteDto>.Ok(leadNote, "Lead note created successfully."));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(ulong leadId, [FromQuery] PaginationQueryDto pagination, CancellationToken cancellationToken)
    {
        var leadNotes = await _getLeadNotesUseCase.ExecuteAsync(leadId, pagination, cancellationToken);
        return Ok(ApiResponse<PaginatedResultDto<LeadNoteDto>>.Ok(leadNotes, "Lead notes fetched successfully."));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(ulong leadId, ulong id, CancellationToken cancellationToken)
    {
        var leadNote = await _getLeadNoteByIdUseCase.ExecuteAsync(leadId, id, cancellationToken);
        return Ok(ApiResponse<LeadNoteDto>.Ok(leadNote, "Lead note fetched successfully."));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(ulong leadId, ulong id, [FromBody] UpdateLeadNoteDto dto, CancellationToken cancellationToken)
    {
        var leadNote = await _updateLeadNoteUseCase.ExecuteAsync(User, leadId, id, dto, cancellationToken);
        return Ok(ApiResponse<LeadNoteDto>.Ok(leadNote, "Lead note updated successfully."));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(ulong leadId, ulong id, CancellationToken cancellationToken)
    {
        await _deleteLeadNoteUseCase.ExecuteAsync(User, leadId, id, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(true, "Lead note deleted successfully."));
    }
}
