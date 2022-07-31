using Domain.Common;

namespace Domain.Entities
{
    public class Category: AuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        //Navigational Properties
        public Guid? CreatedById { get; set; }
        public User CreatedBy { get; set; }
        public ICollection<Book> Books { get; set; }
    }
}
