using System.Security.Claims;
using AutoMapper;
using app.Application.DTOs;
using app.Application.Errors;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Application.UseCases;

public class CreateLeadUseCase
{
    private readonly ILeadRepository _leadRepository;
    private readonly IMapper _mapper;

    public CreateLeadUseCase(ILeadRepository leadRepository, IMapper mapper)
    {
        _leadRepository = leadRepository;
        _mapper = mapper;
    }

    public async Task<LeadDto> ExecuteAsync(ClaimsPrincipal user, CreateLeadDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName))
        {
            throw new ArgumentException(AppErrors.Lead.FirstNameRequired);
        }

        if (string.IsNullOrWhiteSpace(dto.FullName))
        {
            throw new ArgumentException(AppErrors.Lead.FullNameRequired);
        }

        var normalizedPhone = string.IsNullOrWhiteSpace(dto.Phonenumber) ? null : dto.Phonenumber.Trim();
        if (!string.IsNullOrWhiteSpace(normalizedPhone))
        {
            var existed = await _leadRepository.ExistsByPhoneNumberAsync(normalizedPhone, cancellationToken: cancellationToken);
            if (existed)
            {
                throw new InvalidOperationException(AppErrors.Lead.PhoneNumberAlreadyExists);
            }
        }

        var actorUserId = UserClaimResolver.TryGetUserId(user);
        var now = DateTime.UtcNow;
        var lead = new Lead
        {
            FirstName = dto.FirstName.Trim(),
            FullName = dto.FullName.Trim(),
            Source = dto.Source,
            Status = dto.Status,
            Phonenumber = normalizedPhone,
            AssignedTo = dto.AssignedTo,
            Note = dto.Note?.Trim(),
            Metadata = dto.Metadata,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedByUserId = actorUserId,
            UpdatedByUserId = actorUserId
        };

        var created = await _leadRepository.AddAsync(lead, cancellationToken);
        return _mapper.Map<LeadDto>(created);
    }
}
