public class UserDeletedResponse
    {
        public int UserId { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public DateTime DeletedAt { get; set; }
        public required string Message { get; set; }
    }