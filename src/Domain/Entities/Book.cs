using Domain.Common;
using Microsoft.AspNetCore.Http;

namespace Domain.Entities
{
    public class Book : AuditableEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string ShopLink { get; set; }

        //Book-Image
        public Photo? BookImage { get; set; }
        //public IFormFile File { get; set; } goes to dto

        //Navigational Properties
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
