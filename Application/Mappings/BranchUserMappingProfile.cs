using AutoMapper;
using app.Application.DTOs;
using app.Domain.Entities;

namespace app.Application.Mappings;

public class BranchUserMappingProfile : Profile
{
    public BranchUserMappingProfile()
    {
        CreateMap<BranchUser, BranchUserDto>();
    }
}
