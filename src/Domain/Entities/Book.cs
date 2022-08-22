﻿using Domain.Common;
using Microsoft.AspNetCore.Http;

namespace Domain.Entities
{
    public class Book : AuditableEntity
    {
        public Book()
        {
            ReadLists = new List<ReadList>();
            Reviews = new List<Review>();
        }
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string? AuthorDetails { get; set; }
        public string ShopLink { get; set; }
        public string Publisher { get; set; }
        public string? BookFormat { get; set; }
        public string Isbn { get; set; }
        public int? Rating { get; set; }
        public string? BookImageUrl { get; set; }
        public ICollection<ReadList>? ReadLists { get; set; }
        public ICollection<Review>? Reviews { get; set; }

        //Book-Image
        public Photo? BookImage { get; set; }
        //public IFormFile File { get; set; } goes to dto

        //Navigational Properties
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
