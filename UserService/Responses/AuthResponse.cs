namespace UserService.Responses
{
    public class AuthResponse
    {
        public required string Token { get; set; }
        public DateTime Expiration { get; set; }
        public int UserId { get; set; }
        public required string Username { get; set; }
    }
}