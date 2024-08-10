namespace FollowService.Responses;

public class UnfollowResponse
{
    public int FollowId { get; set; }

    public int FollowerId { get; set; }

    public int FolloweeId { get; set; }

    public DateTime UnfollowedAt { get; set; }

    public string Message { get; set; }
}