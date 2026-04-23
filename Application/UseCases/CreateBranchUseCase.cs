using System.Security.Claims;
using AutoMapper;
using app.Application.DTOs;
using app.Application.Errors;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class CreateBranchUseCase
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;

    public CreateBranchUseCase(IBranchRepository branchRepository, IMapper mapper)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
    }

    public async Task<BranchDto> ExecuteAsync(
        ClaimsPrincipal user,
        CreateBranchDto dto,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new ArgumentException(AppErrors.Branch.NameRequired);
        }

        var actorUserId = UserClaimResolver.TryGetUserId(user);
        var now = DateTime.UtcNow;
        var branch = new Branch
        {
            Name = dto.Name.Trim(),
            Description = dto.Description?.Trim(),
            AddressLine1 = dto.AddressLine1?.Trim(),
            AddressLine2 = dto.AddressLine2?.Trim(),
            Ward = dto.Ward?.Trim(),
            District = dto.District?.Trim(),
            City = dto.City?.Trim(),
            PostalCode = dto.PostalCode?.Trim(),
            Country = dto.Country?.Trim(),
            ImageFileId = dto.ImageFileId,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedByUserId = actorUserId,
            UpdatedByUserId = actorUserId
        };

        var created = await _branchRepository.AddAsync(branch, cancellationToken);
        return _mapper.Map<BranchDto>(created);
    }
}
