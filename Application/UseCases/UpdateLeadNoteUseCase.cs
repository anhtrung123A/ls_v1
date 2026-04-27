using System.Security.Claims;
using AutoMapper;
using app.Application.DTOs;
using app.Application.Errors;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class UpdateLeadNoteUseCase
{
    private readonly ILeadRepository _leadRepository;
    private readonly ILeadNoteRepository _leadNoteRepository;
    private readonly IMapper _mapper;

    public UpdateLeadNoteUseCase(ILeadRepository leadRepository, ILeadNoteRepository leadNoteRepository, IMapper mapper)
    {
        _leadRepository = leadRepository;
        _leadNoteRepository = leadNoteRepository;
        _mapper = mapper;
    }

    public async Task<LeadNoteDto> ExecuteAsync(
        ClaimsPrincipal user,
        ulong leadId,
        ulong id,
        UpdateLeadNoteDto dto,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Content))
        {
            throw new ArgumentException(AppErrors.LeadNote.ContentRequired);
        }

        var lead = await _leadRepository.GetByIdAsync(leadId, cancellationToken);
        if (lead is null)
        {
            throw new KeyNotFoundException(AppErrors.Lead.NotFound);
        }

        var existing = await _leadNoteRepository.GetByIdAsync(leadId, id, cancellationToken);
        if (existing is null)
        {
            throw new KeyNotFoundException(AppErrors.LeadNote.NotFound);
        }

        existing.Content = dto.Content.Trim();
        existing.Metadata = dto.Metadata;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedByUserId = UserClaimResolver.TryGetUserId(user);

        var updated = await _leadNoteRepository.UpdateAsync(existing, cancellationToken);
        if (updated is null)
        {
            throw new KeyNotFoundException(AppErrors.LeadNote.NotFound);
        }

        return _mapper.Map<LeadNoteDto>(updated);
    }
}
