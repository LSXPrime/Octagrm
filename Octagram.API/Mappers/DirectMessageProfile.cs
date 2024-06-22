using AutoMapper;
using Octagram.Application.DTOs;
using Octagram.Domain.Entities;

namespace Octagram.API.Mappers;

public class DirectMessageProfile : Profile
{
    public DirectMessageProfile()
    {
        CreateMap<DirectMessage, DirectMessageDto>();
        CreateMap<CreateDirectMessageRequest, DirectMessage>();
    }
}