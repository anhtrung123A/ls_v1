using AutoMapper;
using app.Application.DTOs;
using app.Application.Errors;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class GetLeadNoteByIdUseCase
{
    private readonly ILeadRepository _leadRepository;
    private readonly ILeadNoteRepository _leadNoteRepository;
    private readonly IMapper _mapper;

    public GetLeadNoteByIdUseCase(ILeadRepository leadRepository, ILeadNoteRepository leadNoteRepository, IMapper mapper)
    {
        _leadRepository = leadRepository;
        _leadNoteRepository = leadNoteRepository;
        _mapper = mapper;
    }

    public async Task<LeadNoteDto> ExecuteAsync(ulong leadId, ulong id, CancellationToken cancellationToken = default)
    {
        var lead = await _leadRepository.GetByIdAsync(leadId, cancellationToken);
        if (lead is null)
        {
            throw new KeyNotFoundException(AppErrors.Lead.NotFound);
        }

        var note = await _leadNoteRepository.GetByIdAsync(leadId, id, cancellationToken);
        if (note is null)
        {
            throw new KeyNotFoundException(AppErrors.LeadNote.NotFound);
        }

        return _mapper.Map<LeadNoteDto>(note);
    }
}
