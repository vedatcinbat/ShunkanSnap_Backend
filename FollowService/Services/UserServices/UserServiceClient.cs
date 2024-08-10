using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace FollowService.Services.UserServices
{
    public class UserServiceClient : IUserServiceClient
    {
        private readonly HttpClient _httpClient;

        public UserServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> UserExistsAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/v1/users/{userId}/exists");
                if (!response.IsSuccessStatusCode)
                {
                    return false; // Return false if the response indicates a failure
                }

                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);
                return json["exists"].Value<bool>(); // Return true or false based on the existence check
            }
            catch (HttpRequestException ex)
            {
                // Log exception or handle error accordingly
                Console.WriteLine($"Error checking user existence: {ex.Message}");
                return false;
            }
        }
    }
}


