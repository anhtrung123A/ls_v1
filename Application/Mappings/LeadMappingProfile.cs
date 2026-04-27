using AutoMapper;
using app.Application.DTOs;
using app.Domain.Entities;

namespace app.Application.Mappings;

public class LeadMappingProfile : Profile
{
    public LeadMappingProfile()
    {
        CreateMap<Lead, LeadDto>();
        CreateMap<LeadNote, LeadNoteDto>();
    }
}
