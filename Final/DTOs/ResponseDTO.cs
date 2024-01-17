namespace Final.DTOs
{
    public class ResponseDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Speaker { get; set; }
        public string? Location { get; set; }
        public DateTime? DateRegistered { get; set; } = DateTime.Now;

    }
}
