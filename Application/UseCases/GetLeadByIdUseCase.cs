using AutoMapper;
using app.Application.DTOs;
using app.Application.Errors;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class GetLeadByIdUseCase
{
    private readonly ILeadRepository _leadRepository;
    private readonly IMapper _mapper;

    public GetLeadByIdUseCase(ILeadRepository leadRepository, IMapper mapper)
    {
        _leadRepository = leadRepository;
        _mapper = mapper;
    }

    public async Task<LeadDto> ExecuteAsync(ulong id, CancellationToken cancellationToken = default)
    {
        var lead = await _leadRepository.GetByIdAsync(id, cancellationToken);
        if (lead is null)
        {
            throw new KeyNotFoundException(AppErrors.Lead.NotFound);
        }

        return _mapper.Map<LeadDto>(lead);
    }
}
