using AutoMapper;
using Octagram.Application.DTOs;
using Octagram.Domain.Entities;

namespace Octagram.API.Mappers;

public class FollowProfile : Profile
{
    public FollowProfile()
    {
        CreateMap<Follow, FollowDto>();
    }
}