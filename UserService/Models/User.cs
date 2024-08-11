public class User
{
    public int UserId { get; set; }
    
    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public required string Email { get; set; }

    public required string HashedPassword { get; set; }
    
    public string? ProfilePictureUrl { get; set; }
    public required string Username { get; set; }

    public string? Bio { get; set; }

    public required int Age { get; set; }

    public Gender Gender { get; set; }

    public DateTime DateOfBirth { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    public Visibility Visibility { get; set; }

    public string? ActivationCode { get; set; }

    public bool IsActive { get; set; }

}