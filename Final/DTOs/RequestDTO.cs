using System.Text.Json.Serialization;

namespace Final.DTOs
{
    public class RequestDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Speaker { get; set; }
        public string? Location { get; set; }

    }
}
