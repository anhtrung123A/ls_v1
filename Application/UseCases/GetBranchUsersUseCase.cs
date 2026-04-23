using app.Application.DTOs;
using app.Application.DTOs.Responses;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class GetBranchUsersUseCase
{
    private readonly IBranchUserRepository _branchUserRepository;
    private readonly IFileUrlResolver _fileUrlResolver;

    public GetBranchUsersUseCase(
        IBranchUserRepository branchUserRepository,
        IFileUrlResolver fileUrlResolver)
    {
        _branchUserRepository = branchUserRepository;
        _fileUrlResolver = fileUrlResolver;
    }

    public async Task<PaginatedResultDto<BranchUserDto>> ExecuteAsync(
        ulong? branchId,
        PaginationQueryDto pagination,
        CancellationToken cancellationToken = default)
    {
        var totalRecords = await _branchUserRepository.CountUsersAsync(branchId, cancellationToken);
        var rows = await _branchUserRepository.GetUsersPageAsync(
            branchId,
            pagination.Offset,
            pagination.NormalizedLimit,
            cancellationToken);

        var items = new List<BranchUserDto>(rows.Count);
        foreach (var row in rows)
        {
            var dto = new BranchUserDto
            {
                Id = row.Id,
                Firstname = row.Firstname,
                Lastname = row.Lastname,
                Email = row.Email,
                Phonenumber = row.Phonenumber,
                DateOfBirth = row.DateOfBirth,
                BranchId = row.BranchId,
                Status = row.Status,
                AvatarUrl = _fileUrlResolver.BuildPublicUrl(row.AvatarObjectKey)
            };
            items.Add(dto);
        }

        return new PaginatedResultDto<BranchUserDto>
        {
            Items = items,
            TotalRecords = totalRecords,
            CurrentPage = pagination.NormalizedPage,
            Limit = pagination.NormalizedLimit,
            Offset = pagination.Offset
        };
    }
}
