using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Tokens;
using UserService.Responses;
using UserService.Configuration;

namespace UserService.Services.UserService;

public class UserService : IUserService
{
    private readonly UserDbContext _context;
    private readonly UserEventPublisher _eventPublisher;
    private readonly JwtSettings _jwtSettings;

    public UserService(UserDbContext context, UserEventPublisher eventPublisher, JwtSettings jwtSettings)
    {
        _context = context;
        _eventPublisher = eventPublisher;
        _jwtSettings = jwtSettings;
    }

    public async Task<AuthResponse> AuthenticateAsync(string username, string password)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

        if (user == null || !VerifyPassword(user.HashedPassword, password))
        {
            return null;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Key); // Ensure this key is at least 32 bytes long

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, "User")
            }),
            Expires = DateTime.UtcNow.AddDays(_jwtSettings.ExpireDays),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return new AuthResponse
        {
            Token = tokenString,
            Expiration = tokenDescriptor.Expires ?? DateTime.UtcNow.AddDays(_jwtSettings.ExpireDays),
            UserId = user.UserId,
            Username = user.Username
        };
    }
    
    public async Task<User> CreateUserAsync(CreateUserRequest request)
    {
        string hashedPassword = HashPassword(request.Password);

        User user = new()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            HashedPassword = hashedPassword,
            ProfilePictureUrl = request.ProfilePictureUrl,
            Username = request.Username,
            Bio = request.Bio,
            Age = request.Age,
            Gender = request.Gender,
            DateOfBirth = request.DateOfBirth,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false,
            Visibility = request.Visibility
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _eventPublisher.PublishUserCreated(user);

        return user;
    }

    public async Task<User?> DeleteUserAsync(int userId)
    {
        var user = await _context.Users.Where(u => u.IsDeleted == false).SingleOrDefaultAsync(x => x.UserId == userId);

        if (user == null)
        {
            return null;
        }

        user.IsDeleted = true;
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _eventPublisher.PublishUserDeleted(user);

        return user;
    }

    public async Task<bool> GetUserByUsernameAsync(string username)
    {
        return await _context.Users.AnyAsync(x => x.Username == username);
    }

    public async Task<bool> GetUserByEmailAsync(string email)
    {
        return await _context.Users.AnyAsync(x => x.Email == email);
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users.SingleOrDefaultAsync(x => x.UserId == userId);
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
    }

    private bool VerifyPassword(string hashedPassword, string password)
    {
        return hashedPassword == HashPassword(password);
    }
}