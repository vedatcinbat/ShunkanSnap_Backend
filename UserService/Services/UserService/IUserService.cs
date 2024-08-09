public interface IUserService {
    Task<User> CreateUserAsync(CreateUserRequest request);
    Task<bool> GetUserByUsernameAsync(string username);
    Task<bool> GetUserByEmailAsync(string username);
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int userId);
    Task<User?> DeleteUserAsync(int userId);
}