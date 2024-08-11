namespace UserService.Responses;

public class UserSavedResponse
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public DateTime DeletedAt { get; set; }
    public required string Message { get; set; }
}