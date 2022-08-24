namespace Application.DataTransferObjects
{
    public record ViewReviewDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }

    public record ReviewDTO
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public Guid BookId { get; set; }
    }

    public record CreateReviewDTO
    {
        public string Description { get; set; }
    }
}
