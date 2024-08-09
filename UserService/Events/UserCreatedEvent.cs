namespace UserService.Events;

public class UserCreatedEvent
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public DateTime timestamp { get; set; } = DateTime.UtcNow;
    public int UserId { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}
