using AutoMapper;
using app.Application.DTOs;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class GetBranchesUseCase
{
    private readonly IBranchRepository _branchRepository;
    private readonly IFileUrlResolver _fileUrlResolver;
    private readonly IMapper _mapper;

    public GetBranchesUseCase(IBranchRepository branchRepository, IFileUrlResolver fileUrlResolver, IMapper mapper)
    {
        _branchRepository = branchRepository;
        _fileUrlResolver = fileUrlResolver;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<BranchDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var branches = await _branchRepository.GetAllAsync(cancellationToken);
        var results = new List<BranchDto>(branches.Count);
        foreach (var branch in branches)
        {
            var dto = _mapper.Map<BranchDto>(branch);
            if (branch.ImageFileId.HasValue)
            {
                var imageObjectKey = await _branchRepository.GetImageObjectKeyByFileIdAsync(branch.ImageFileId.Value, cancellationToken);
                dto.ImageUrl = _fileUrlResolver.BuildPublicUrl(imageObjectKey);
            }

            results.Add(dto);
        }

        return results;
    }
}
