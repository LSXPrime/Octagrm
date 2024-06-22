namespace Octagram.Application.DTOs;

public class JwtTokenDto
{
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
}