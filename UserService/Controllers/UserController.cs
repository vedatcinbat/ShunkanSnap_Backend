using Microsoft.AspNetCore.Mvc;
using UserService.Responses;
using UserService.Services;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType<CreateUserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        if (await UsernameExistsAsync(request.Username))
        {
            return BadRequest(new ProblemDetail {
                ProblemType = "username_already_exists",
                Status = StatusCodes.Status400BadRequest,
                Title = "Username already exists.",
                Detail = "The username provided already exists. Please provide a different username."
            });
        }

        if(await EmailExistsAsync(request.Email))
        {
            return BadRequest(new ProblemDetail {
                ProblemType = "email_already_exists",
                Status = StatusCodes.Status400BadRequest,
                Title = "Email already exists.",
                Detail = "The email provided already exists. Please provide a different email."
            });
        }

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

    private async Task<bool> UsernameExistsAsync(string username)
    {
        bool isUsernameExists = await _userService.GetUserByUsernameAsync(username);

        return isUsernameExists;
    }

    private async Task<bool> EmailExistsAsync(string email)
    {
        var isEmaileExists = await _userService.GetUserByEmailAsync(email);

        return isEmaileExists;
    }
}

