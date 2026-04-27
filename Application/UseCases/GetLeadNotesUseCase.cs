using AutoMapper;
using app.Application.DTOs;
using app.Application.DTOs.Responses;
using app.Application.Errors;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class GetLeadNotesUseCase
{
    private readonly ILeadRepository _leadRepository;
    private readonly ILeadNoteRepository _leadNoteRepository;
    private readonly IMapper _mapper;

    public GetLeadNotesUseCase(ILeadRepository leadRepository, ILeadNoteRepository leadNoteRepository, IMapper mapper)
    {
        _leadRepository = leadRepository;
        _leadNoteRepository = leadNoteRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResultDto<LeadNoteDto>> ExecuteAsync(ulong leadId, PaginationQueryDto pagination, CancellationToken cancellationToken = default)
    {
        var lead = await _leadRepository.GetByIdAsync(leadId, cancellationToken);
        if (lead is null)
        {
            throw new KeyNotFoundException(AppErrors.Lead.NotFound);
        }

        var totalRecords = await _leadNoteRepository.CountByLeadIdAsync(leadId, cancellationToken);
        var notes = await _leadNoteRepository.GetPageByLeadIdAsync(leadId, pagination.Offset, pagination.NormalizedLimit, cancellationToken);

        return new PaginatedResultDto<LeadNoteDto>
        {
            Items = _mapper.Map<IReadOnlyList<LeadNoteDto>>(notes),
            TotalRecords = totalRecords,
            CurrentPage = pagination.NormalizedPage,
            Limit = pagination.NormalizedLimit,
            Offset = pagination.Offset
        };
    }
}
