using AutoMapper;
using app.Application.DTOs;
using app.Application.DTOs.Responses;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class GetAssignedLeadsByUserUseCase
{
    private readonly ILeadRepository _leadRepository;
    private readonly IMapper _mapper;

    public GetAssignedLeadsByUserUseCase(ILeadRepository leadRepository, IMapper mapper)
    {
        _leadRepository = leadRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResultDto<LeadDto>> ExecuteAsync(ulong userId, PaginationQueryDto pagination, CancellationToken cancellationToken = default)
    {
        var totalRecords = await _leadRepository.CountAssignedToAsync(userId, cancellationToken);
        var leads = await _leadRepository.GetAssignedPageAsync(userId, pagination.Offset, pagination.NormalizedLimit, cancellationToken);

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
