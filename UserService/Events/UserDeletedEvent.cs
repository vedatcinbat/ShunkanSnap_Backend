namespace UserService.Events;

public class UserDeletedEvent
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int UserId { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public DateTime DeletedAt { get; set; }
}
