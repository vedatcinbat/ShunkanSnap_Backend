using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FollowService.Data;
using FollowService.Models;
using FollowService.Responses;
using FollowService.Services.UserServices;
using UserService.Responses;

namespace FollowService.Controllers
{
    [ApiController]
    [Route("api/v1/follows")]
    public class FollowController : ControllerBase
    {
        private readonly FollowDbContext _context;

        private readonly IUserServiceClient _userServiceClient;

        public FollowController(FollowDbContext context, IUserServiceClient userServiceClient)
        {
            _context = context;
            _userServiceClient = userServiceClient;
        }

        [HttpPost("{followerId}/followed/{followeeId}")]
public async Task<IActionResult> FollowUser(int followerId, int followeeId)
{
    if (followerId == followeeId)
    {
        return BadRequest(new ProblemDetail
        {
            ProblemType = "follower-and-followee-userid-same",
            Title = "Follower and Followee UserId Same",
            Detail = "FollowerUserId and FolloweeUserId can't be the same.",
            Status = 400
        });
    }

    if (!await _userServiceClient.UserExistsAsync(followerId))
    {
        return NotFound(new ProblemDetail
        {
            ProblemType = "follower-user-not-found",
            Title = "Follower User Not Found",
            Detail = $"Follower User with Id {followerId} not found.",
            Status = 404
        });
    }

    if (!await _userServiceClient.UserExistsAsync(followeeId))
    {
        return NotFound(new ProblemDetail
        {
            ProblemType = "followee-user-not-found",
            Title = "Followee User Not Found",
            Detail = $"Followee User with Id {followeeId} not found.",
            Status = 404
        });
    }

    var existingFollow = await _context.Follows
        .FirstOrDefaultAsync(f => f.FollowerUserId == followerId && f.FolloweeUserId == followeeId);

    if (existingFollow != null)
    {
        if (existingFollow.IsDeleted)
        {
            existingFollow.IsDeleted = false;
            existingFollow.FollowedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var responseAgain = new CreateFollowResponse
            {
                FollowId = existingFollow.FollowId,
                FollowerUserId = existingFollow.FollowerUserId,
                FolloweeUserId = existingFollow.FolloweeUserId,
                FollowedAt = existingFollow.FollowedAt,
                Message = "Followed user again successfully."
            };

            return Ok(responseAgain);
        }
        
        return BadRequest(new ProblemDetail
        {
            ProblemType = "follower-already-following",
            Title = "Already Following",
            Detail = "The follower is already following the followee.",
            Status = 400
        });
    }

    var follow = new Follow
    {
        FollowerUserId = followerId,
        FolloweeUserId = followeeId,
        FollowedAt = DateTime.UtcNow,
        IsDeleted = false
    };

    _context.Follows.Add(follow);
    await _context.SaveChangesAsync();

    var response = new CreateFollowResponse
    {
        FollowId = follow.FollowId,
        FollowerUserId = follow.FollowerUserId,
        FolloweeUserId = follow.FolloweeUserId,
        FollowedAt = follow.FollowedAt,
        Message = "Followed user successfully."
    };

    return Ok(response);
}


        [HttpDelete("{followerId}/unfollow/{followeeId}")]
        public async Task<IActionResult> UnfollowUser(int followerId, int followeeId)
        {
            var follow = await _context.Follows
                .FirstOrDefaultAsync(f => f.FollowerUserId == followerId && f.FolloweeUserId == followeeId);
    
            if (follow == null)
            {
                return NotFound(new ProblemDetail()
                {
                    ProblemType = "relationship-not-found",
                    Title = "Relationship Not FOUND",
                    Detail = $"",
                    Status = 404
                });
            }

            follow.IsDeleted = true;
            follow.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var response = new UnfollowResponse
            {
                FollowId = follow.FollowId,
                FollowerId = follow.FollowerUserId,
                FolloweeId = follow.FolloweeUserId,
                UnfollowedAt = DateTime.UtcNow,
                Message = "Unfollowed user successfully."
            };

            return Ok(response);
        }
    }
}
