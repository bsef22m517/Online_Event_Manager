using EventHub.Models;
using EventHubApplication.Data;
using EventHubApplication.Models.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace EventHubApplication.Models.Repositories
{

    public class EventRepository : IEventRepository
    {
        ApplicationDbContext _context;
        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Event> GetAll()
        {
            return _context.Events.ToList();
        }

        public void AddEvent(Event e)
        {
            _context.Events.Add(e);
            _context.SaveChanges();
        }
        public Event GetEvent(int Id)
        {
            return _context.Events.FirstOrDefault(ev => ev.Id==Id);
        }
        public Event GetEvent(int id, string organizerId)
        {
            return _context.Events.FirstOrDefault(ev => ev.Id == id && ev.OrganizerId == organizerId);
        }

        public void DeleteEvent(int id, string organizerId)
        {
            var e = _context.Events.FirstOrDefault(ev => ev.Id == id && ev.OrganizerId == organizerId);
            if (e != null)
            {
                _context.Events.Remove(e);
                _context.SaveChanges();
            }
        }
        public List<RSVP> GetRSVPsForOrganizer(string organizerId)
        {
            var myEventIds = _context.Events.Where(e => e.OrganizerId == organizerId).Select(e => e.Id).ToList();

            var rsvps = _context.RSVPs.Where(r => myEventIds.Contains(r.EventId)).ToList();

            var users = _context.Users.ToList();
            var events = _context.Events.Where(e => myEventIds.Contains(e.Id)).ToList();

            foreach (var rsvp in rsvps)
            {
                rsvp.Event = events.FirstOrDefault(e => e.Id == rsvp.EventId);
                rsvp.User = users.FirstOrDefault(u => u.Id == rsvp.UserId);
            }

            return rsvps;
        }

        public void UpdateEvent(Event updatedEvent)
        {
            var existingEvent = _context.Events.FirstOrDefault(ev => ev.Id == updatedEvent.Id && ev.OrganizerId == updatedEvent.OrganizerId);

            if (existingEvent != null)
            {
                existingEvent.Title = updatedEvent.Title;
                existingEvent.Date = updatedEvent.Date;
                existingEvent.Speaker = updatedEvent.Speaker;
                existingEvent.Type = updatedEvent.Type;
                existingEvent.Location = updatedEvent.Location;
                existingEvent.Description = updatedEvent.Description;
                existingEvent.ImageUrl = updatedEvent.ImageUrl;
                _context.SaveChanges();
            }
        }
        public List<Event> GetEventsByIds(List<int> events)
        {
            return _context.Events.Where(e => events.Contains(e.Id)).ToList();
        }
        public List<RSVP> GetRSVPsByEventId(int eventId)
        {
            var rsvps = _context.RSVPs.ToList();
            var r = new List<RSVP>();

            foreach (var rsvp in rsvps)
            {
                if (rsvp.EventId == eventId)
                {
                    rsvp.User = _context.Users.FirstOrDefault(u => u.Id == rsvp.UserId);
                    r.Add(rsvp);
                }
            }

            return r;
        }

        public void RSVPToEvent(string userId, int eventId)
        {
            var rsvps = _context.RSVPs.ToList();
            bool already = false;

            foreach (var rsvp in rsvps)
            {
                if (rsvp.UserId == userId && rsvp.EventId == eventId)
                {
                    already = true;
                    break;
                }
            }

            if (!already)
            {
                _context.RSVPs.Add(new RSVP { UserId = userId, EventId = eventId });
                _context.SaveChanges();
            }
        }

        public List<Event> GetAllEventsByOrganizer(string organizerId)
        {
            return _context.Events.Where(e => e.OrganizerId == organizerId).ToList();
        }


        public List<Event> GetEventsByAttendee(string userId)
        {
            var r = new List<Event>();
            var rsvps = _context.RSVPs.ToList();
            var allevents = _context.Events.ToList();

            foreach (var rsvp in rsvps)
            {       
                if (rsvp.UserId == userId)
                {
                    var mEvent = allevents.FirstOrDefault(e => e.Id == rsvp.EventId);
                    if (mEvent != null)
                    {
                        r.Add(mEvent);
                    }
                }

            }

            return r;
        }

        public List<RSVP> GetAllRSVPs()
        {
            return _context.RSVPs.ToList();
        }

        public bool HasUserRSVPed(string userId, int eventId)
        {
            var rsvps = _context.RSVPs.ToList();
            foreach (var rsvp in rsvps)
            {
                if (rsvp.UserId == userId && rsvp.EventId == eventId)
                {
                    return true;
                }
            }


            return false;
        }

        public RSVP GetUserRSVP(string userId, int eventId)
        {
            return _context.RSVPs.FirstOrDefault(r => r.UserId == userId && r.EventId == eventId);
        }

    }
}
