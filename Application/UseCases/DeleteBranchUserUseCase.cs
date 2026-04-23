using System.Security.Claims;
using app.Application.Errors;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class DeleteBranchUserUseCase
{
    private readonly IBranchUserRepository _branchUserRepository;

    public DeleteBranchUserUseCase(IBranchUserRepository branchUserRepository)
    {
        _branchUserRepository = branchUserRepository;
    }

    public async Task ExecuteAsync(
        ClaimsPrincipal user,
        ulong id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await _branchUserRepository.SoftDeleteAsync(
            id,
            UserClaimResolver.TryGetUserId(user),
            cancellationToken);

        if (!deleted)
        {
            throw new KeyNotFoundException(AppErrors.BranchUser.NotFound);
        }
    }
}
