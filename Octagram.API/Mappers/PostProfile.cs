using AutoMapper;
using Octagram.Application.DTOs;
using Octagram.Domain.Entities;

namespace Octagram.API.Mappers;

public class PostProfile : Profile
{
    public PostProfile()
    {
        CreateMap<Post, PostDto>()
            .ForMember(dest => dest.LikesCount, opt => opt.MapFrom(src => src.Likes.Count))
            .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments));

        CreateMap<CreatePostRequest, Post>();
        CreateMap<UpdatePostRequest, Post>();
    }
}