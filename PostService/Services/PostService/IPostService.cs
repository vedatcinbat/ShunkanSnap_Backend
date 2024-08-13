using PostService.Models;

namespace PostService.Services.PostService;

public interface IPostService
{
    Task<List<Post>> QueryPostsAsync();
    
    Task<Post> CreatePostAsync(CreatePostRequest post);
}