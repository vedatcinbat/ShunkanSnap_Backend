namespace UserService.Services.FollowService;

public interface IFollowServiceClient
{
    Task<IEnumerable<int>> GetFollowersIdsWithFolloweeUserId(int userId);
}