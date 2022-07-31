

namespace Domain.Entities
{
    public class Photo
    {
        public Guid Id { get; set; }
        public byte[] Bytes { get; set; }
        public string? Description { get; set; }
        public string FileExtension { get; set; }
        public decimal Size { get; set; }

        //Navigational Property
        public Guid BookId { get; set; }
        public Book Book { get; set; }
    }
}
