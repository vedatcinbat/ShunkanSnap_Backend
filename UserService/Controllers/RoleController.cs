using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Enums;
using UserService.Models;
using UserService.Requests;

namespace UserService.Controllers;

[ApiController]
[Route("api/v1/roles")]
public class RoleController : ControllerBase
{
    private readonly UserDbContext _context;
    
    public RoleController(UserDbContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
    {
        var roles = await _context.Roles.ToListAsync();
        return Ok(roles);
    }
}