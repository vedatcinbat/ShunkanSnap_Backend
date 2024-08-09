public interface IUserService {
    Task<User> CreateUserAsync(CreateUserRequest request);
    Task<bool> GetUserByUsernameAsync(string username);
    Task<bool> GetUserByEmailAsync(string username);
}