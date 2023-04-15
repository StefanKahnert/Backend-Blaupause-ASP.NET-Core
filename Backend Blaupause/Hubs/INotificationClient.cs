using System.Threading.Tasks;

namespace Backend_Blaupause.Hubs
{
    public interface INotificationClient
    {
        Task NotificateAll(string message);
    }
}
