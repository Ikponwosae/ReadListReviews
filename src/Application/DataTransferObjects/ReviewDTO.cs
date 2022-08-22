namespace Application.DataTransferObjects
{
    public record ViewReviewDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
    }
}
