using AutoMapper;
using Octagram.Application.DTOs;
using Octagram.Domain.Entities;

namespace Octagram.API.Mappers;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<Comment, CommentDto>();
        CreateMap<CreateCommentRequest, Comment>();
    }
}