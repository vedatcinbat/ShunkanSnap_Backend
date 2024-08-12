namespace UserService.Responses;

public class UserRoleUpdatedResponse
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public DateTime UpdatedAt { get; set; }
}