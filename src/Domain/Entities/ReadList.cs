using Domain.Common;

namespace Domain.Entities
{
    public class ReadList : AuditableEntity
    {
        public ReadList()
        {
            Books = new List<Book>();
        }
        public Guid Id { get; set; }
        private string _name { get; set; }
        public string Name { get { return _name; } set { _name = value; } }

        //Navigational Properties
        public Guid UserId { get; set; }
        public User Owner { get; set; }
        public ICollection<Book> Books { get; set; }
    }
}
