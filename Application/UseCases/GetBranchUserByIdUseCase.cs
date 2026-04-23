using app.Application.DTOs;
using app.Application.Errors;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class GetBranchUserByIdUseCase
{
    private readonly IBranchUserRepository _branchUserRepository;
    private readonly IFileUrlResolver _fileUrlResolver;

    public GetBranchUserByIdUseCase(
        IBranchUserRepository branchUserRepository,
        IFileUrlResolver fileUrlResolver)
    {
        _branchUserRepository = branchUserRepository;
        _fileUrlResolver = fileUrlResolver;
    }

    public async Task<BranchUserDto> ExecuteAsync(ulong id, CancellationToken cancellationToken = default)
    {
        var row = await _branchUserRepository.GetUserByBranchUserIdAsync(id, cancellationToken);
        if (row is null)
        {
            throw new KeyNotFoundException(AppErrors.BranchUser.NotFound);
        }

        return new BranchUserDto
        {
            Id = row.Value.Id,
            Firstname = row.Value.Firstname,
            Lastname = row.Value.Lastname,
            Email = row.Value.Email,
            Phonenumber = row.Value.Phonenumber,
            DateOfBirth = row.Value.DateOfBirth,
            BranchId = row.Value.BranchId,
            Status = row.Value.Status,
            AvatarUrl = _fileUrlResolver.BuildPublicUrl(row.Value.AvatarObjectKey)
        };
    }
}
