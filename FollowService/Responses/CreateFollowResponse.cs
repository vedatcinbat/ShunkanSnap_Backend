namespace FollowService.Responses
{
    public class CreateFollowResponse
    {
        public int FollowId { get; set; }

        public int FollowerUserId { get; set; }

        public int FolloweeUserId { get; set; }

        public DateTime FollowedAt { get; set; }

        public string? Message { get; set; }
    }
}

