using app.Application.DTOs;
using app.Application.DTOs.Responses;
using app.Application.UseCases;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace app.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Authorize(Policy = "BranchCrud")]
[Route("api/v{version:apiVersion}/branches")]
public class BranchesController : ControllerBase
{
    private readonly CreateBranchUseCase _createBranchUseCase;
    private readonly GetBranchesUseCase _getBranchesUseCase;
    private readonly GetBranchByIdUseCase _getBranchByIdUseCase;
    private readonly UpdateBranchUseCase _updateBranchUseCase;
    private readonly DeleteBranchUseCase _deleteBranchUseCase;
    private readonly UpsertBranchImageUseCase _upsertBranchImageUseCase;

    public BranchesController(
        CreateBranchUseCase createBranchUseCase,
        GetBranchesUseCase getBranchesUseCase,
        GetBranchByIdUseCase getBranchByIdUseCase,
        UpdateBranchUseCase updateBranchUseCase,
        DeleteBranchUseCase deleteBranchUseCase,
        UpsertBranchImageUseCase upsertBranchImageUseCase)
    {
        _createBranchUseCase = createBranchUseCase;
        _getBranchesUseCase = getBranchesUseCase;
        _getBranchByIdUseCase = getBranchByIdUseCase;
        _updateBranchUseCase = updateBranchUseCase;
        _deleteBranchUseCase = deleteBranchUseCase;
        _upsertBranchImageUseCase = upsertBranchImageUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBranchDto dto, CancellationToken cancellationToken)
    {
        var branch = await _createBranchUseCase.ExecuteAsync(User, dto, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<BranchDto>.Ok(branch, "Branch created successfully."));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var branches = await _getBranchesUseCase.ExecuteAsync(cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<BranchDto>>.Ok(branches, "Branches fetched successfully."));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(ulong id, CancellationToken cancellationToken)
    {
        var branch = await _getBranchByIdUseCase.ExecuteAsync(id, cancellationToken);
        return Ok(ApiResponse<BranchDto>.Ok(branch, "Branch fetched successfully."));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(ulong id, [FromBody] UpdateBranchDto dto, CancellationToken cancellationToken)
    {
        var branch = await _updateBranchUseCase.ExecuteAsync(User, id, dto, cancellationToken);
        return Ok(ApiResponse<BranchDto>.Ok(branch, "Branch updated successfully."));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(ulong id, CancellationToken cancellationToken)
    {
        await _deleteBranchUseCase.ExecuteAsync(User, id, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(true, "Branch deleted successfully."));
    }

    [HttpPost("{id:long}/image")]
    public async Task<IActionResult> UploadImage(ulong id, [FromForm] UploadBranchImageDto dto, CancellationToken cancellationToken)
    {
        var image = await _upsertBranchImageUseCase.ExecuteAsync(User, id, dto.Image, cancellationToken);
        return Ok(ApiResponse<BranchImageDto>.Ok(image, "Branch image uploaded successfully."));
    }

    [HttpPut("{id:long}/image")]
    public async Task<IActionResult> EditImage(ulong id, [FromForm] UploadBranchImageDto dto, CancellationToken cancellationToken)
    {
        var image = await _upsertBranchImageUseCase.ExecuteAsync(User, id, dto.Image, cancellationToken);
        return Ok(ApiResponse<BranchImageDto>.Ok(image, "Branch image updated successfully."));
    }
}
