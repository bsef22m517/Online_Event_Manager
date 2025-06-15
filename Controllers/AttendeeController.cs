using System.Security.Claims;
using EventHubApplication.Hubs;
using EventHubApplication.Models;
using EventHubApplication.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace EventHubApplication.Controllers
{
    public class AttendeeController : Controller
    {

        private readonly IEventRepository _eventRepository;
        private readonly IHubContext<Hubs.NotificationHub> _hubContext;
        public AttendeeController(IEventRepository eventRepository, IHubContext<Hubs.NotificationHub> hubContext)
        {
            _eventRepository = eventRepository;
            _hubContext = hubContext;
        }
        [Authorize(Policy = "AttendeeAllowed")]
       
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var allEvents = _eventRepository.GetAll();

            var model = allEvents.Select(e => new AttendeeViewModel
            {
                Event = e,
                HasRSVPed = _eventRepository.HasUserRSVPed(userId, e.Id)
            }).ToList();
            ViewBag.Name =User.Identity.Name;
            return View(model);
        }

        [Authorize(Policy = "AttendeeAllowed")]
        public async Task<IActionResult> MyEvents()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var rsvpevents= _eventRepository.GetEventsByAttendee(userId);
          
            return View(rsvpevents);
        }
        [Authorize(Policy = "AttendeeAllowed")]

        [HttpGet]
        public IActionResult EventDetails(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var ev = _eventRepository.GetEvent(id);

            if (ev == null)
                return NotFound();

            var rsvp = _eventRepository.GetUserRSVP(userId, id);

            var model = new AttendeeViewModel
            {
                Event = ev,
                HasRSVPed = rsvp != null,
                RSVPDate = rsvp?.RSVPDate
            };

            return View(model);
        }

        [Authorize(Policy = "AttendeeAllowed")]
        public async Task<IActionResult> RSVP(int id)
        {
            var ev = _eventRepository.GetEvent(id);
            return View(ev);
        }

        [HttpPost]
        [Authorize(Policy = "AttendeeAllowed")]
        public async Task<IActionResult> RSVPConfirm(int eventId)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _eventRepository.RSVPToEvent(userId, eventId);
            Event e = _eventRepository.GetEvent(eventId);
            string e_rganizer = e.OrganizerId;
            await _hubContext.Clients.User(e_rganizer).SendAsync("ReceiveRSVPNotification", $"User {userId} RSVP'd to event {eventId}");
            return RedirectToAction("EventDetails", new { id = eventId });
        }
    }
}
