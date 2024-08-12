using UserService.Enums;

namespace UserService.Models;

public class Role
{
    public int RoleId { get; set; }
    
    public required UserRole RoleName { get; set; }
}