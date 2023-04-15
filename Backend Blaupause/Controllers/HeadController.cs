using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Backend_Blaupause.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class HeadController : ControllerBase
    {
        [HttpHead]
        [MapToApiVersion("1.0")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> Head()
        {
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        [HttpHead]
        [MapToApiVersion("2.0")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Head20()
        {
            return Ok("Server is up");
        }

    }
}
