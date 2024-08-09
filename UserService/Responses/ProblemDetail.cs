namespace UserService.Responses
{
    public class ProblemDetail
    {
        public required string ProblemType { get; set; }
        public int Status { get; set; }
        public required string Title { get; set; }
        public required string Detail { get; set; }
    }
}