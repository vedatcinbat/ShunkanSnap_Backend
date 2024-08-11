using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Requests;
using UserService.Responses;
using UserService.Services;
using UserService.Services.FollowService;

namespace UserService.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;
    private readonly UserDbContext _context;
    private readonly UserEventPublisher _eventPublisher;
    private readonly IFollowServiceClient _followService;
    
    public UserController(IUserService userService, IFollowServiceClient followService ,ILogger<UserController> logger, UserDbContext context, UserEventPublisher eventPublisher)
    {
        _userService = userService;
        _logger = logger;
        _context = context;
        _eventPublisher = eventPublisher;
        _followService = followService;
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
                ProfilePictureUrl = user.ProfilePictureUrl,
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
    public async Task<IActionResult> GetUserProfile(int userId)
{
    var user = await _userService.GetUserByIdAsync(userId);
    
    if (user == null)
    {
        return NotFound(new ProblemDetail
        {
            ProblemType = "user-not-found",
            Title = "User Not Found",
            Status = StatusCodes.Status404NotFound,
            Detail = $"User with ID {userId} does not exist."
        });
    }
    
    var currentUserIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    
    if (currentUserIdClaim == null)
    {
        if (user.Visibility == Visibility.Private)
        {
            return BadRequest(new ProblemDetail
            {
                ProblemType = "profile-is-private-user-is-not-logged-in",
                Title = "User has not authenticated and profile is private",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Profile is private and user is not logged in"
            });
        }
        
        if (user.Visibility == Visibility.Public)
        {
            var responsePublicVis = new UserResponse
            {
                UserId = user.UserId,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Bio = user.Bio,
                Age = user.Age,
                Gender = user.Gender,
                Visibility = user.Visibility
            };
            
            return Ok(responsePublicVis);
        }
    }
    else
    {
        if (!int.TryParse(currentUserIdClaim, out var currentUserId))
        {
            return BadRequest(new ProblemDetail
            {
                ProblemType = "invalid-user-id-claim",
                Title = "Invalid User ID Claim",
                Status = StatusCodes.Status400BadRequest,
                Detail = "The User ID claim is invalid."
            });
        }
        
        var isCurrentUserFollows = await _followService.IsCurrentUserFollowsThisPerson(currentUserId, userId);
        
        if (!isCurrentUserFollows)
        {
            if (user.Visibility == Visibility.Private)
            {
                return BadRequest(new ProblemDetail
                {
                    ProblemType = "profile-is-private-user-not-followed",
                    Title = "Profile is private and current user is not following",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = "Profile is private and you are not following this user."
                });
            }
        }

        var response = new UserResponse
        {
            UserId = user.UserId,
            ProfilePictureUrl = user.ProfilePictureUrl,
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

    return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetail
    {
        ProblemType = "unexpected-error",
        Title = "Unexpected Error",
        Status = StatusCodes.Status500InternalServerError,
        Detail = "An unexpected error occurred while processing the request."
    });
}


    [HttpPatch("update-visibility")]
    public async Task<IActionResult> UpdateUserVisibility(UpdateVisibilityRequest request)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null)
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
            var userId = int.Parse(userIdClaim);

            var user = await _context.Users.Where(u => u.IsDeleted == false).SingleOrDefaultAsync(u => u.UserId == userId);

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

            if (user.Visibility == request.Visibility)
            {
                return Ok($"User has already same visibility situation: {request.Visibility}");
            }

            user.Visibility = request.Visibility;
            user.UpdatedAt = DateTime.UtcNow;
        
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        
            var response = new UserVisibilityChangedResponse()
            {
                UserId = user.UserId,
                Username = user.Username,
                Visibility = user.Visibility,
                UpdatedAt = user.UpdatedAt
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the user.");
            return StatusCode(500, new { Message = "An error occurred while creating the user.", Error = ex.Message });
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

    [HttpPost("save-account")]
    public async Task<IActionResult> SaveUserAccount()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        
        if (userIdClaim == null)
        {
            return Unauthorized(new ProblemDetail
            {
                ProblemType = "unauthorized_access",
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized access.",
                Detail = "You have to authenticate first"
            });
        }
        
        try
        {
            var userId = int.Parse(userIdClaim);

            var user = await _context.Users.SingleOrDefaultAsync(u => u.UserId == userId);

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

            if (user.IsDeleted == false)
            {
                return BadRequest(new ProblemDetail
                {
                    ProblemType = "user_has_already_active",
                    Status = StatusCodes.Status400BadRequest,
                    Title = "User has already active.",
                    Detail = "The user with the provided user id has already active."
                });
            }

            user.IsDeleted = false;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _eventPublisher.PublishUserSaved(user);
            
            var response = new UserSavedResponse()
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                DeletedAt = user.UpdatedAt,
                Message = $"User {user.UserId} {user.Username} {user.LastName} saved successfully."
            };

            return Ok(response);
        }catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting the user.");
            return StatusCode(500, new { Message = "An error occurred while saving the user.", Error = ex.Message });
        }
        
    }

    [HttpGet("get-followers")]
    public async Task<IActionResult> GetFollowers()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null)
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
            var followerIds = await _followService.GetFollowersIdsWithFolloweeUserId(int.Parse(userIdClaim));

            var followers = await _context.Users
                .Where(u => followerIds.Contains(u.UserId) && !u.IsDeleted)
                .Select(u => new
                {
                    u.UserId,
                    u.Username,
                    u.Bio,
                    u.ProfilePictureUrl
                }).ToListAsync();

            return Ok(followers);
        }catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting the user.");
            return StatusCode(500, new { Message = "An error occurred while saving the user.", Error = ex.Message });
        }
    }

    [HttpDelete("delete-user/{userId}")]
    [Authorize]
    public async Task<IActionResult> DeleteUser([FromRoute] int userId)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null || userId != int.Parse(userIdClaim))
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

    [HttpGet("{id}/exists")]
    public async Task<IActionResult> UserExists(int id)
    {
        var exists = await _context.Users.Where(u => u.IsDeleted == false).AnyAsync(u => u.UserId == id);
        return Ok(new { exists });
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

