namespace Domain.Entities
{
    public class Token
    {
        public Token()
        {
            CreatedAt = DateTime.UtcNow;
        }
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string TokenType { get; set; }
        public string Value { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(4);
    }
}
