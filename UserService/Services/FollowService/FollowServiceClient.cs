using Newtonsoft.Json;
using UserService.Configuration;

namespace UserService.Services.FollowService;

public class FollowServiceClient : IFollowServiceClient
{
    private readonly UserDbContext _context;
    private readonly UserEventPublisher _eventPublisher;
    private readonly JwtSettings _jwtSettings;
    private readonly HttpClient _httpClient;

    public FollowServiceClient(UserDbContext context, UserEventPublisher eventPublisher, JwtSettings jwtSettings, HttpClient httpClient)
    {
        _context = context;
        _eventPublisher = eventPublisher;
        _jwtSettings = jwtSettings;
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<int>> GetFollowersIdsWithFolloweeUserId(int userId)
    {
        var response = await _httpClient.GetAsync($"/api/v1/follows/get-user-follower-ids/{userId}");

        var content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<IEnumerable<int>>(content);    
    }

    public async Task<bool> IsCurrentUserFollowsThisPerson(int currentUserId, int followeeUserId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/v1/follows/{currentUserId}/isFollows/{followeeUserId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return bool.TryParse(content, out var isFollowing) && isFollowing;
            }
            
            return false;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
            return false;
        }
    }
}