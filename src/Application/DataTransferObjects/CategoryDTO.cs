namespace Application.DataTransferObjects
{
    public record CategoryDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
