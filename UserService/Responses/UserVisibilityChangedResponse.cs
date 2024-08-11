namespace UserService.Responses;

public class UserVisibilityChangedResponse
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    public Visibility Visibility { get; set; }
    public DateTime UpdatedAt { get; set; }
}