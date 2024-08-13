using PostService.Enums;

namespace PostService.Models;

public class Post
{
    public int PostId { get; set; }
    
    public int UserId { get; set; }
    
    public string? Caption { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    public int LikesCount { get; set; }
    
    public int CommentsCount { get; set; }
    
    public PostActivityStatus IsActive { get; set; }
    
    public PostVisibility Visibility { get; set; }
    
    public PostType PostType { get; set; }
    
    public int? MusicTrackId { get; set; }
    
    public int? LocationId { get; set; }
}