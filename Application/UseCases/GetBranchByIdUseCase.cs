using AutoMapper;
using app.Application.DTOs;
using app.Application.Errors;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class GetBranchByIdUseCase
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;

    public GetBranchByIdUseCase(IBranchRepository branchRepository, IMapper mapper)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
    }

    public async Task<BranchDto> ExecuteAsync(ulong id, CancellationToken cancellationToken = default)
    {
        var branch = await _branchRepository.GetByIdAsync(id, cancellationToken);
        if (branch is null)
        {
            throw new KeyNotFoundException(AppErrors.Branch.NotFound);
        }

        return _mapper.Map<BranchDto>(branch);
    }
}
