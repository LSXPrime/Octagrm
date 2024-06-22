using AutoMapper;
using Octagram.Application.DTOs;
using Octagram.Domain.Entities;

namespace Octagram.API.Mappers;

public class StoryProfile : Profile
{
    public StoryProfile()
    {
        CreateMap<Story, StoryDto>();
        CreateMap<CreateStoryRequest, Story>(); 
    }
}