using System.Security.Claims;
using app.Application.Errors;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class DeleteLeadUseCase
{
    private readonly ILeadRepository _leadRepository;

    public DeleteLeadUseCase(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task ExecuteAsync(ClaimsPrincipal user, ulong id, CancellationToken cancellationToken = default)
    {
        var deleted = await _leadRepository.SoftDeleteAsync(id, UserClaimResolver.TryGetUserId(user), cancellationToken);
        if (!deleted)
        {
            throw new KeyNotFoundException(AppErrors.Lead.NotFound);
        }
    }
}
