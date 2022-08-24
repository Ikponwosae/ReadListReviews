using Application.Helpers;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace Application.DataTransferObjects
{
    public record ViewBookDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
    }

    public record BookDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string? AuthorDetails { get; set; }
        public string? ShopLink { get; set; }
        public string Publisher { get; set; }
        public string? BookFormat { get; set; }
        public string Isbn { get; set; }
        public int? Rating { get; set; }
        public BookImageDTO BookImage { get; set; }
        public string? BookImageUrl { get; set; }
        public ICollection<ReviewDTO>? Reviews { get; set; }
        public CreateCategoryDTO Category { get; set; }
    }

    public record BookImageDTO
    {
        public Guid Id { get; set; }
    }

    public record FetchBooksDTO
    {
        //[JsonPropertyName("status")]
        //public string Status { get; set; }

        //[JsonPropertyName("copyright")]
        //public string Copyright { get; set; }

        [JsonPropertyName("num_results")]
        public long NumResults { get; set; }

        [JsonPropertyName("results")]
        public Results Results { get; set; }
    }

    public record Results
    {
        [JsonPropertyName("lists")]
        public List<BestList> BestLists { get; set; }
    }

    public record BestList
    {

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }

        [JsonPropertyName("books")]
        public List<FetchBook> Books { get; set; }
    }

    public record FetchBook
    {
        [JsonPropertyName("amazon_product_url")]
        public string ShopLink { get; set; }

        [JsonPropertyName("author")]
        public string Author { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("book_image")]
        public string BookImageUrl { get; set; }

        [JsonPropertyName("primary_isbn13")]
        public string Isbn { get; set; }

        [JsonPropertyName("publisher")]
        public string Publisher { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }
    }

    public record AddBookDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string? AuthorDetails { get; set; }
        public string? ShopLink { get; set; }
        public string Publisher { get; set; }
        public string? BookFormat { get; set; }
        public string Isbn { get; set; }
        public int? Rating { get; set; }
        public string? BookImageUrl { get; set; }
    }
    
    public record UpdateBookDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string? AuthorDetails { get; set; }
        public string? ShopLink { get; set; }
        public string Publisher { get; set; }
        public string? BookFormat { get; set; }
        public string Isbn { get; set; }
        public int? Rating { get; set; }
        public string? BookImageUrl { get; set; }
        public IFormFile? BookImage { get; set; }
    }

    public record SearchBooksDTO
    {
        public PagedList<ViewBookDTO> Books { get; set; }
        public PagedList<CreateCategoryDTO> Categories { get; set; }
    }
}
