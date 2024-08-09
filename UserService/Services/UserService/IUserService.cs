public interface IUserService {
    Task<User> CreateUserAsync(CreateUserRequest request);
}