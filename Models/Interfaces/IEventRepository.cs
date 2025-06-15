using EventHub.Models;
using EventHubApplication.Data;

namespace EventHubApplication.Models.Interfaces
{
    public interface IEventRepository
    {
        // ORGANIZER CAN DO ALL THIS
        public void AddEvent(Event e);
        public Event GetEvent(int id, string organizerId);
        public Event GetEvent(int id);
        public void DeleteEvent(int id, string organizerId);
        public List<Event> GetAllEventsByOrganizer(string organizerId);
        public List<Event> GetEventsByIds(List<int> events);
        public void UpdateEvent(Event e);
        public List<Event> GetAll();
        //USER CAN DO ALL THIS 
        public List<RSVP> GetAllRSVPs();
        public List<RSVP> GetRSVPsByEventId(int eventId);

        public List<RSVP> GetRSVPsForOrganizer(string organizerId);

        public void RSVPToEvent(string userId, int eventId);
        public List<Event> GetEventsByAttendee(string userId);
        public bool HasUserRSVPed(string userId, int eventId);
        public RSVP GetUserRSVP(string userId, int eventId);

    }
}
