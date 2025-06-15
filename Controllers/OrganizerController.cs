using System.Security.Claims;
using EventHubApplication.Hubs;
using EventHubApplication.Models;
using EventHubApplication.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace EventHubApplication.Controllers
{
    public class OrganizerController : Controller
    {
        private readonly IEventRepository _eventRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHubContext<NotificationHub> _hubContext;
        public OrganizerController(IEventRepository eventRepository, UserManager<AppUser> userManager, IHubContext<Hubs.NotificationHub> hubContext)
        {
            _eventRepository = eventRepository;
            _userManager = userManager;
            _hubContext = hubContext;
        }
        [Authorize(Policy = "OrganizerAllowed")]
        public async Task<IActionResult> Dashboard()
        {
               
            string organizerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
 
            var events = _eventRepository.GetAllEventsByOrganizer(organizerId);

            var viewModel = new OrganizerViewModel
            {

                Name = User.Identity.Name,
                
                Events = events
            };

            return View(viewModel);
        }
        [Authorize(Policy = "OrganizerAllowed")]

        public async Task<IActionResult> EventDetails(int id)
        {
            string organizerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var eventItem = _eventRepository.GetEvent(id, organizerId);
            return View(eventItem);
        }
        [Authorize(Policy = "OrganizerAllowed")]

        public async Task<IActionResult> AllRSVPs(int id)
        {
            var rsvps = _eventRepository.GetRSVPsByEventId(id);
            return View(rsvps);
        }
        public async Task<IActionResult> AllRSVPsByOrganizer()
        {
            string organizerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var rsvps = _eventRepository.GetRSVPsForOrganizer(organizerId);
            return View(rsvps);
        }


        [HttpGet]
        [Authorize(Policy = "OrganizerAllowed")]

        public async Task<IActionResult> AddEvent()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "OrganizerAllowed")]

        public async Task<IActionResult> AddEvent(string title, string description, string speaker, string type, DateTime date, string location, string imageUrl)
        {
            string organizerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Event @event = new Event
            {
                Title = title,
                Description = description,
                Speaker = speaker,
                Type = type,
                Date = date,
                Location = location,
                OrganizerId =organizerId,
                ImageUrl= imageUrl

            };

            _eventRepository.AddEvent(@event);
            await _hubContext.Clients.All.SendAsync("ReceiveEventUpdate", $"New Event Added: {@event.Title}");
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        [Authorize(Policy = "OrganizerAllowed")]

        public async Task<IActionResult> EditEvent(int id)
        {
            string organizerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var eventItem = _eventRepository.GetEvent(id, organizerId);
            return View(eventItem);
        }

        [HttpPost]
        [Authorize(Policy = "OrganizerAllowed")]
        public async Task<IActionResult> EditEvent(Event updatedEvent)
        {
            string organizerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            updatedEvent.OrganizerId = organizerId;
            _eventRepository.UpdateEvent(updatedEvent);
            await _hubContext.Clients.All.SendAsync("ReceiveEventUpdate", $"Event '{updatedEvent.Title}' was updated.");
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        [Authorize(Policy = "OrganizerAllowed")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            string organizerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var eventItem = _eventRepository.GetEvent(id, organizerId);
            return View(eventItem);
        }

        [HttpPost, ActionName("DeleteEvent")]
        [Authorize(Policy = "OrganizerAllowed")]
        public async Task<IActionResult> ConfirmDeleteEvent(int id)
        {
            string organizerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _eventRepository.DeleteEvent(id, organizerId);
            await _hubContext.Clients.All.SendAsync("ReceiveEventUpdate", $"An Event was deleted.");
            return RedirectToAction("Dashboard");
        }
    }
}
