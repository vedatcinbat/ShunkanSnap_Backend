using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase {
    [HttpGet]
    public IActionResult Get() {
        return Ok("Get all users");
    }
}