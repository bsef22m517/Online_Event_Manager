using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace EventHubApplication.Hubs
{
    
    public class NotificationHub : Hub
    {
        public async Task SendEventUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveEventUpdate", message);
        }

        public async Task SendRSVPNotification(string organizerUserId, string message)
        {
            await Clients.User(organizerUserId).SendAsync("ReceiveRSVPNotification", message);
        }
    }

}
