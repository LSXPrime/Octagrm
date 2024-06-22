using AutoMapper;
using Octagram.Application.DTOs;
using Octagram.Domain.Entities;

namespace Octagram.API.Mappers;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.FollowersCount, opt => opt.MapFrom(src => src.Followers.Count))
            .ForMember(dest => dest.FollowingCount, opt => opt.MapFrom(src => src.Following.Count)); 

        CreateMap<UpdateUserRequest, User>();
    }
}