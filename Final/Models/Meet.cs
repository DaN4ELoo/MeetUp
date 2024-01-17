namespace Final.Models
{
    public class Meet
    {

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Speaker { get; set; }
        public string? Location { get; set; } 
        public DateTime? DateRegistered { get; set; } = DateTime.Now;

    }
}
