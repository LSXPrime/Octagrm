using Microsoft.AspNetCore.Http;

namespace Octagram.Application.DTOs;

public class CreateStoryRequest
{
    public IFormFile MediaFile { get; set; }
    public string MediaType { get; set; } // "image" or "video"
}