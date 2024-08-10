namespace FollowService.Services.UserServices
{
    public interface IUserServiceClient
    {
        Task<bool> UserExistsAsync(int userId);
    }
}

