using AutoMapper;
using Octagram.Application.DTOs;
using Octagram.Domain.Entities;

namespace Octagram.API.Mappers;

public class LikeProfile : Profile
{
    public LikeProfile()
    {
        CreateMap<Like, LikeDto>(); 
    }
}