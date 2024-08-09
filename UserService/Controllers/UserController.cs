using Microsoft.AspNetCore.Mvc;
using UserService.Services;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserEventPublisher _publisher;
    private readonly IUserService _userService;

    private readonly ILogger<UserController> _logger;

    public UserController(UserEventPublisher publisher, IUserService userService, ILogger<UserController> logger)
    {
        _publisher = publisher;
        _userService = userService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType<CreateUserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            var user = await _userService.CreateUserAsync(request);

            var response = new CreateUserResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                Message = "User created successfully."
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the user.");
            return StatusCode(500, new { Message = "An error occurred while creating the user.", Error = ex.Message });
        }


    }
}