using UserService.Enums;

namespace UserService.Responses;

public class UserAuthInfosResponse
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    
    public required string UserRole { get; set; }
}