using AutoMapper;
using app.Application.DTOs;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class GetBranchesUseCase
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;

    public GetBranchesUseCase(IBranchRepository branchRepository, IMapper mapper)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<BranchDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var branches = await _branchRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<BranchDto>>(branches);
    }
}
