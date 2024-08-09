using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Requests;
using UserService.Responses;

[ApiController]
[Route("api/v1/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet("get-all-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();

            var response = users.Select(user => new UserResponse()
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Bio = user.Bio,
                Age = user.Age,
                Gender = user.Gender,
                Visibility = user.Visibility
            });

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching the users.");
            return StatusCode(500, new { Message = "An error occurred while fetching the users.", Error = ex.Message });
        }
    }

    [HttpGet("get-user/{userId}")]
    public async Task<IActionResult> GetUserById(int userId)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new ProblemDetail
                {
                    ProblemType = "user_not_found",
                    Status = StatusCodes.Status404NotFound,
                    Title = "User not found.",
                    Detail = "The user with the provided user id does not exist."
                });
            }

            var response = new UserResponse
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Bio = user.Bio,
                Age = user.Age,
                Gender = user.Gender,
                Visibility = user.Visibility
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching the user.");
            return StatusCode(500, new { Message = "An error occurred while fetching the user.", Error = ex.Message });
        }
    }

    [HttpPost("create-user")]
    [ProducesResponseType<CreateUserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        if (await UsernameExistsAsync(request.Username))
        {
            return BadRequest(new ProblemDetail
            {
                ProblemType = "username_already_exists",
                Status = StatusCodes.Status400BadRequest,
                Title = "Username already exists.",
                Detail = "The username provided already exists. Please provide a different username."
            });
        }

        if (await EmailExistsAsync(request.Email))
        {
            return BadRequest(new ProblemDetail
            {
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

    [HttpDelete("delete-user/{userId}")]
    [Authorize]
    public async Task<IActionResult> DeleteUser([FromRoute] int userId)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if(userIdClaim == null || userId != int.Parse(userIdClaim))
        {
            return Unauthorized(new ProblemDetail
            {
                ProblemType = "unauthorized_access",
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized access.",
                Detail = "You are not authorized to delete this user."
            });
        }

        try
        {
            var user = await _userService.DeleteUserAsync(userId);

            if (user == null)
            {
                return NotFound(new ProblemDetail
                {
                    ProblemType = "user_not_found",
                    Status = StatusCodes.Status404NotFound,
                    Title = "User not found.",
                    Detail = "The user with the provided user id does not exist."
                });
            }

            var response = new UserDeletedResponse()
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                DeletedAt = user.UpdatedAt,
                Message = "User deleted successfully."
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting the user.");
            return StatusCode(500, new { Message = "An error occurred while deleting the user.", Error = ex.Message });
        }
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var authResponse = await _userService.AuthenticateAsync(request.Username, request.Password);

        if (authResponse == null)
        {
            return BadRequest(new { Message = "Username or password is incorrect." });
        }

        return Ok(authResponse);
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

