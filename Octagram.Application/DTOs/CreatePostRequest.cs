using Microsoft.AspNetCore.Http;

namespace Octagram.Application.DTOs;

public class CreatePostRequest
{
    public IFormFile ImageFile { get; set; }
    public string Caption { get; set; }
    public List<string> Hashtags { get; set; }
}