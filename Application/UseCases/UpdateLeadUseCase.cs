using System.Security.Claims;
using AutoMapper;
using app.Application.DTOs;
using app.Application.Errors;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class UpdateLeadUseCase
{
    private readonly ILeadRepository _leadRepository;
    private readonly IMapper _mapper;

    public UpdateLeadUseCase(ILeadRepository leadRepository, IMapper mapper)
    {
        _leadRepository = leadRepository;
        _mapper = mapper;
    }

    public async Task<LeadDto> ExecuteAsync(ClaimsPrincipal user, ulong id, UpdateLeadDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName))
        {
            throw new ArgumentException(AppErrors.Lead.FirstNameRequired);
        }

        if (string.IsNullOrWhiteSpace(dto.FullName))
        {
            throw new ArgumentException(AppErrors.Lead.FullNameRequired);
        }

        var existing = await _leadRepository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            throw new KeyNotFoundException(AppErrors.Lead.NotFound);
        }

        existing.FirstName = dto.FirstName.Trim();
        existing.FullName = dto.FullName.Trim();
        existing.Source = dto.Source;
        existing.Status = dto.Status;
        existing.AssignedTo = dto.AssignedTo;
        existing.Note = dto.Note?.Trim();
        existing.Metadata = dto.Metadata;
        existing.UpdatedAt = DateTime.UtcNow;
        existing.UpdatedByUserId = UserClaimResolver.TryGetUserId(user);

        var updated = await _leadRepository.UpdateAsync(existing, cancellationToken);
        if (updated is null)
        {
            throw new KeyNotFoundException(AppErrors.Lead.NotFound);
        }

        return _mapper.Map<LeadDto>(updated);
    }
}
