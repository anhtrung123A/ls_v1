using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using app.Common.Responses;
using app.DTOs.CourseCategories;
using app.Repositories.CourseCategories;

namespace app.Controllers;

[ApiController]
[Route("api/course_categories")]
[Authorize]
public class CourseCategoriesController : ControllerBase
{
    private readonly ICourseCategoryRepository _repo;
    public CourseCategoriesController(ICourseCategoryRepository repo) { _repo = repo; }

    [HttpGet]
    public async Task<IResult> List([FromQuery] CourseCategoryListQuery query, CancellationToken cancellationToken)
    {
        var result = await _repo.GetAllAsync(query, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Get course categories successfully."));
    }

    [HttpGet("{id:long}")]
    public async Task<IResult> Detail(long id, CancellationToken cancellationToken)
    {
        EnsureAdminPermission();
        var result = await _repo.GetByIdAsync(id, cancellationToken);
        if (result is null) throw new KeyNotFoundException("Course category not found.");
        return Results.Ok(ApiResponse.Ok(result, "Get course category successfully."));
    }

    [HttpPost]
    public async Task<IResult> Create([FromBody] CourseCategoryRequest request, CancellationToken cancellationToken)
    {
        EnsureAdminPermission();
        var result = await _repo.CreateAsync(request, cancellationToken);
        return Results.Ok(ApiResponse.Ok(result, "Create course category successfully."));
    }

    [HttpPut("{id:long}")]
    public async Task<IResult> Update(long id, [FromBody] CourseCategoryRequest request, CancellationToken cancellationToken)
    {
        EnsureAdminPermission();
        var result = await _repo.UpdateAsync(id, request, cancellationToken);
        if (result is null) throw new KeyNotFoundException("Course category not found.");
        return Results.Ok(ApiResponse.Ok(result, "Update course category successfully."));
    }

    [HttpDelete("{id:long}")]
    public async Task<IResult> Delete(long id, CancellationToken cancellationToken)
    {
        EnsureAdminPermission();
        await _repo.DeleteAsync(id, cancellationToken);
        return Results.Ok(ApiResponse.Ok(null, "Delete course category successfully."));
    }

    private void EnsureAdminPermission()
    {
        var role = User.FindFirst("role")?.Value;
        if (role != "1") throw new UnauthorizedAccessException("Only admin can manage course categories.");
    }
}
