using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace UserService.Services;

public class UserService : IUserService
{
    private readonly UserDbContext _context;
    private readonly UserEventPublisher _eventPublisher;


    public UserService(UserDbContext context, UserEventPublisher eventPublisher)
    {
        _context = context;
        _eventPublisher = eventPublisher;
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
        var user = await _context.Users.SingleOrDefaultAsync(x => x.UserId == userId);

        if(user == null)
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
}