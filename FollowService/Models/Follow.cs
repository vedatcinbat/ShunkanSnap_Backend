using Microsoft.EntityFrameworkCore;

namespace FollowService.Models;

public class Follow {
    public int FollowId { get; set; }

    public int FollowerUserId { get; set; }

    public int FolloweeUserId { get; set; }

    public DateTime FollowedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }
}