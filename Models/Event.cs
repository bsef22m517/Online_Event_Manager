namespace EventHubApplication.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public string Speaker { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public string OrganizerId { get; set; }
        public string? ImageUrl { get; set; }

    }
}
