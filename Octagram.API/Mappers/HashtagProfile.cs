using AutoMapper;
using Octagram.Application.DTOs;
using Octagram.Domain.Entities;

namespace Octagram.API.Mappers;

public class HashtagProfile : Profile
{
    public HashtagProfile()
    {
        CreateMap<Hashtag, HashtagDto>(); 
    }
}