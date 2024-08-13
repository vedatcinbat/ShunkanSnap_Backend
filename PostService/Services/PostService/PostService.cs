using Microsoft.EntityFrameworkCore;
using PostService.Data;
using PostService.Enums;
using PostService.Models;

namespace PostService.Services.PostService;

public class PostService : IPostService
{
    private readonly PostDbContext _dbContext;
    
    public PostService(PostDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<List<Post>> QueryPostsAsync()
    {
        var posts = await _dbContext.Posts.AsNoTracking().ToListAsync();
        return posts;
    }
    
    public async Task<Post> CreatePostAsync(CreatePostRequest post)
    {
        // If MusicTrackId is not null, check if it exists in the database
        // _musicService.GetMusicTrackByIdAsync(post.MusicTrackId);
        // Update the responses with the actual values from the database
        // For example return MusicTrackId, MusicTrackName, MusicTrackUrl in the response
        // If LocationId is not null, check if it exists in the database
        // _locationService.GetLocationByIdAsync(post.LocationId);
        // Update the responses with the actual values from the database
        // For example return LocationId, LocationName, LocationAddress in the response
        
        return new Post()
        {
            UserId = post.UserId,
            Caption = post.Caption,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            LikesCount = 0,
            CommentsCount = 0,
            IsActive = PostActivityStatus.Active,
            Visibility = PostVisibility.Public,
            PostType = PostType.Image,
            MusicTrackId = post.MusicTrackId,
            LocationId = post.LocationId
        };            
    }
}