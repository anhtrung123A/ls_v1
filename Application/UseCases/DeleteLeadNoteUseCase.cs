using System.Security.Claims;
using app.Application.Errors;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class DeleteLeadNoteUseCase
{
    private readonly ILeadRepository _leadRepository;
    private readonly ILeadNoteRepository _leadNoteRepository;

    public DeleteLeadNoteUseCase(ILeadRepository leadRepository, ILeadNoteRepository leadNoteRepository)
    {
        _leadRepository = leadRepository;
        _leadNoteRepository = leadNoteRepository;
    }

    public async Task ExecuteAsync(ClaimsPrincipal user, ulong leadId, ulong id, CancellationToken cancellationToken = default)
    {
        var lead = await _leadRepository.GetByIdAsync(leadId, cancellationToken);
        if (lead is null)
        {
            throw new KeyNotFoundException(AppErrors.Lead.NotFound);
        }

        var deleted = await _leadNoteRepository.SoftDeleteAsync(leadId, id, UserClaimResolver.TryGetUserId(user), cancellationToken);
        if (!deleted)
        {
            throw new KeyNotFoundException(AppErrors.LeadNote.NotFound);
        }
    }
}
