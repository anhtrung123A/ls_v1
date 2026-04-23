using System.Security.Claims;
using AutoMapper;
using app.Application.DTOs;
using app.Application.Errors;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class UpdateBranchUseCase
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;

    public UpdateBranchUseCase(IBranchRepository branchRepository, IMapper mapper)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
    }

    public async Task<BranchDto> ExecuteAsync(
        ClaimsPrincipal user,
        ulong id,
        UpdateBranchDto dto,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new ArgumentException(AppErrors.Branch.NameRequired);
        }

        var existing = await _branchRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            throw new KeyNotFoundException(AppErrors.Branch.NotFound);
        }

        existing.Name = dto.Name.Trim();
        existing.Description = dto.Description?.Trim();
        existing.AddressLine1 = dto.AddressLine1?.Trim();
        existing.AddressLine2 = dto.AddressLine2?.Trim();
        existing.Ward = dto.Ward?.Trim();
        existing.District = dto.District?.Trim();
        existing.City = dto.City?.Trim();
        existing.PostalCode = dto.PostalCode?.Trim();
        existing.Country = dto.Country?.Trim();
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedByUserId = UserClaimResolver.TryGetUserId(user);

        var updated = await _branchRepository.UpdateAsync(existing, cancellationToken);
        if (updated is null)
        {
            throw new KeyNotFoundException(AppErrors.Branch.NotFound);
        }

        return _mapper.Map<BranchDto>(updated);
    }
}
