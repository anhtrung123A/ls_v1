using AutoMapper;
using app.Application.DTOs;
using app.Application.Errors;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class GetBranchByIdUseCase
{
    private readonly IBranchRepository _branchRepository;
    private readonly IFileUrlResolver _fileUrlResolver;
    private readonly IMapper _mapper;

    public GetBranchByIdUseCase(IBranchRepository branchRepository, IFileUrlResolver fileUrlResolver, IMapper mapper)
    {
        _branchRepository = branchRepository;
        _fileUrlResolver = fileUrlResolver;
        _mapper = mapper;
    }

    public async Task<BranchDto> ExecuteAsync(ulong id, CancellationToken cancellationToken = default)
    {
        var branch = await _branchRepository.GetByIdAsync(id, cancellationToken);
        if (branch is null)
        {
            throw new KeyNotFoundException(AppErrors.Branch.NotFound);
        }

        var dto = _mapper.Map<BranchDto>(branch);
        if (branch.ImageFileId.HasValue)
        {
            var imageObjectKey = await _branchRepository.GetImageObjectKeyByFileIdAsync(branch.ImageFileId.Value, cancellationToken);
            dto.ImageUrl = _fileUrlResolver.BuildPublicUrl(imageObjectKey);
        }

        return dto;
    }
}
