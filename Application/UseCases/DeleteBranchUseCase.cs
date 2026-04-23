using System.Security.Claims;
using app.Application.Errors;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class DeleteBranchUseCase
{
    private readonly IBranchRepository _branchRepository;

    public DeleteBranchUseCase(IBranchRepository branchRepository)
    {
        _branchRepository = branchRepository;
    }

    public async Task ExecuteAsync(
        ClaimsPrincipal user,
        ulong id,
        CancellationToken cancellationToken = default)
    {
        var deleted = await _branchRepository.SoftDeleteAsync(
            id,
            UserClaimResolver.TryGetUserId(user),
            cancellationToken);

        if (!deleted)
        {
            throw new KeyNotFoundException(AppErrors.Branch.NotFound);
        }
    }
}
