namespace EventHubApplication.Models
{
    public class AttendeeViewModel
    {
       
            public Event Event { get; set; }
            public bool HasRSVPed { get; set; }
        public DateTime? RSVPDate { get; set; }

    }
}
