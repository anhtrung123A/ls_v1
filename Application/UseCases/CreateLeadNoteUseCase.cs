using System.Security.Claims;
using AutoMapper;
using app.Application.DTOs;
using app.Application.Errors;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class CreateLeadNoteUseCase
{
    private readonly ILeadRepository _leadRepository;
    private readonly ILeadNoteRepository _leadNoteRepository;
    private readonly IMapper _mapper;

    public CreateLeadNoteUseCase(ILeadRepository leadRepository, ILeadNoteRepository leadNoteRepository, IMapper mapper)
    {
        _leadRepository = leadRepository;
        _leadNoteRepository = leadNoteRepository;
        _mapper = mapper;
    }

    public async Task<LeadNoteDto> ExecuteAsync(ClaimsPrincipal user, ulong leadId, CreateLeadNoteDto dto, CancellationToken cancellationToken = default)
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

        var actorUserId = UserClaimResolver.TryGetUserId(user);
        var now = DateTime.UtcNow;
        var leadNote = new LeadNote
        {
            LeadId = leadId,
            Content = dto.Content.Trim(),
            Metadata = dto.Metadata,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedByUserId = actorUserId,
            UpdatedByUserId = actorUserId
        };

        var created = await _leadNoteRepository.AddAsync(leadNote, cancellationToken);
        return _mapper.Map<LeadNoteDto>(created);
    }
}
