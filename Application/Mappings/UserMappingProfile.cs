using AutoMapper;
using app.Application.DTOs;
using app.Domain.Entities;

namespace app.Application.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<User, BranchUserDto>();
    }
}
