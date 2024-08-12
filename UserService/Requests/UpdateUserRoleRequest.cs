namespace UserService.Requests;

public class UpdateUserRoleRequest
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
}