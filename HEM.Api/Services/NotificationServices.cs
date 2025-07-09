using HEM.Api.Hubs;
using HEM.Api.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace HEM.Api.Services
{
    public class NotificationServices: INotificationService
    {

        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationServices(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task SendNotificationToAllAsync(string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}
