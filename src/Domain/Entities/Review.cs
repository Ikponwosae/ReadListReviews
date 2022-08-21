using Domain.Enums;
using Domain.Common;

namespace Domain.Entities
{
    public class Review : AuditableEntity
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = EReviewStatus.Draft.ToString();

        //Navigational properties
        public Guid UserId { get; set; }
        public User Owner { get; set; }
        public Guid BookId { get; set; }
        public Book Book { get; set; }

    }
}
