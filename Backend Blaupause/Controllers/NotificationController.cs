using Backend_Blaupause.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Net;
using System.Threading.Tasks;

namespace Backend_Blaupause.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class NotificationController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _notificationHub;

        public NotificationController(IHubContext<NotificationHub> notificationHub)
        {
            _notificationHub = notificationHub;
        }

        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> NotificateAll([FromQuery] string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return BadRequest($"{nameof(message)} is mandatory as query parameter");
            }

            await _notificationHub.Clients.All.SendAsync("Notify", message);

            return Ok("Message Sended");
        }

    }
}
