namespace Application.DataTransferObjects
{
    public record CategoryDTO
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public IEnumerable<ViewBookDTO> Books { get; set; }
    }

    public record CreateCategoryDTO
    {
        public string Name { get; set; }
    }
}
