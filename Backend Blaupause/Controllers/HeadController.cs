using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Backend_Blaupause.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HeadController : ControllerBase
    {
        [HttpHead]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> head()
        {
            return StatusCode((int)HttpStatusCode.NoContent);
        }

    }
}
