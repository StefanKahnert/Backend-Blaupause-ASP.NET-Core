using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Backend_Blaupause.Hubs
{
    public class NotificationHub : Hub<INotificationClient>
    {
        public async Task SendMessage(string message)
        {
            await Clients.All.NotificateAll(message);
        }
          
    }
}
