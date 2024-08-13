using Microsoft.AspNetCore.Mvc;
using PostService.Responses;
using PostService.Services.PostService;

namespace PostService.Controllers;

[ApiController]
[Route("/api/posts")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;
    
    public PostController(IPostService postService)
    {
        _postService = postService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetPostsAsync()
    {
        var posts = await _postService.QueryPostsAsync();
        var queryPostsResponse = posts.Select(post => new CreatePostResponse()
        {
            PostId = post.PostId,
            UserId = post.UserId,
            Caption = post.Caption,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            LikesCount = post.LikesCount,
            CommentsCount = post.CommentsCount,
            IsActive = post.IsActive,
            Visibility = post.Visibility,
            PostType = post.PostType,
            MusicTrackId = post.MusicTrackId,
            LocationId = post.LocationId
        });
        return Ok(queryPostsResponse);
    }
}