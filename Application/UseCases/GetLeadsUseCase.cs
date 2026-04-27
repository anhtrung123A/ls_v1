using AutoMapper;
using app.Application.DTOs;
using app.Application.DTOs.Responses;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class GetLeadsUseCase
{
    private readonly ILeadRepository _leadRepository;
    private readonly IMapper _mapper;

    public GetLeadsUseCase(ILeadRepository leadRepository, IMapper mapper)
    {
        _leadRepository = leadRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResultDto<LeadDto>> ExecuteAsync(PaginationQueryDto pagination, CancellationToken cancellationToken = default)
    {
        var totalRecords = await _leadRepository.CountAsync(cancellationToken);
        var leads = await _leadRepository.GetPageAsync(pagination.Offset, pagination.NormalizedLimit, cancellationToken);

        return new PaginatedResultDto<LeadDto>
        {
            Items = _mapper.Map<IReadOnlyList<LeadDto>>(leads),
            TotalRecords = totalRecords,
            CurrentPage = pagination.NormalizedPage,
            Limit = pagination.NormalizedLimit,
            Offset = pagination.Offset
        };
    }
}
