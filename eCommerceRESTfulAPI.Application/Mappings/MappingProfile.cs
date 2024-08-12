using AutoMapper;
using eCommerceRESTfulAPI.Application.DTOs;
using eCommerceRESTfulAPI.Domain.Entities;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<User, UserUpdateDto>().ReverseMap();
        CreateMap<User, UserCreateDto>().ReverseMap();
    }
}
