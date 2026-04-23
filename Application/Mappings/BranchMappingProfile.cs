using AutoMapper;
using app.Application.DTOs;
using app.Domain.Entities;

namespace app.Application.Mappings;

public class BranchMappingProfile : Profile
{
    public BranchMappingProfile()
    {
        CreateMap<Branch, BranchDto>();
    }
}
