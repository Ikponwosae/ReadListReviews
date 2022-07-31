using Domain.Common;

namespace Domain.Entities
{
    public class ReadList : AuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        //Navigational Properties
        public Guid UserId { get; set; }
        public User Owner { get; set; }
        public ICollection<Book> Books { get; set; }
    }
}
