public class CreateUserRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    
    public string? ProfilePictureUrl { get; set; }
    public required string Username { get; set; }
    public required string? Bio { get; set; }
    public int Age { get; set; }
    public required Gender Gender { get; set; }
    public required DateTime DateOfBirth { get; set; }
    public required Visibility Visibility { get; set; }
}