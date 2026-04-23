using app.Application.DTOs;
using app.Application.DTOs.Responses;
using app.Application.UseCases;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace app.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Authorize]
[Route("api/v{version:apiVersion}/branch-users")]
public class BranchUsersController : ControllerBase
{
    private readonly CreateBranchUserUseCase _createBranchUserUseCase;
    private readonly GetBranchUsersUseCase _getBranchUsersUseCase;
    private readonly GetBranchUserByIdUseCase _getBranchUserByIdUseCase;
    private readonly UpdateBranchUserUseCase _updateBranchUserUseCase;
    private readonly DeleteBranchUserUseCase _deleteBranchUserUseCase;

    public BranchUsersController(
        CreateBranchUserUseCase createBranchUserUseCase,
        GetBranchUsersUseCase getBranchUsersUseCase,
        GetBranchUserByIdUseCase getBranchUserByIdUseCase,
        UpdateBranchUserUseCase updateBranchUserUseCase,
        DeleteBranchUserUseCase deleteBranchUserUseCase)
    {
        _createBranchUserUseCase = createBranchUserUseCase;
        _getBranchUsersUseCase = getBranchUsersUseCase;
        _getBranchUserByIdUseCase = getBranchUserByIdUseCase;
        _updateBranchUserUseCase = updateBranchUserUseCase;
        _deleteBranchUserUseCase = deleteBranchUserUseCase;
    }

    [Authorize(Policy = "BranchCrud")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBranchUserDto dto, CancellationToken cancellationToken)
    {
        var created = await _createBranchUserUseCase.ExecuteAsync(User, dto, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, ApiResponse<BranchUserDto>.Ok(created, "Branch user created successfully."));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] ulong? branchId,
        [FromQuery] PaginationQueryDto pagination,
        CancellationToken cancellationToken)
    {
        var branchUsers = await _getBranchUsersUseCase.ExecuteAsync(branchId, pagination, cancellationToken);
        return Ok(ApiResponse<PaginatedResultDto<BranchUserDto>>.Ok(branchUsers, "Branch users fetched successfully."));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(ulong id, CancellationToken cancellationToken)
    {
        var branchUser = await _getBranchUserByIdUseCase.ExecuteAsync(id, cancellationToken);
        return Ok(ApiResponse<BranchUserDto>.Ok(branchUser, "Branch user fetched successfully."));
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(ulong id, [FromBody] UpdateBranchUserDto dto, CancellationToken cancellationToken)
    {
        var updated = await _updateBranchUserUseCase.ExecuteAsync(User, id, dto, cancellationToken);
        return Ok(ApiResponse<BranchUserDto>.Ok(updated, "Branch user updated successfully."));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(ulong id, CancellationToken cancellationToken)
    {
        await _deleteBranchUserUseCase.ExecuteAsync(User, id, cancellationToken);
        return Ok(ApiResponse<bool>.Ok(true, "Branch user deleted successfully."));
    }
}
