using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Backend_Blaupause.Controllers
{
    [ApiController]
    [Route("")]
    public class HeadEndpoint : ControllerBase
    {
        [HttpHead]
        public async Task<IActionResult> head()
        {
            return StatusCode((int)HttpStatusCode.NoContent);
        }
    }
}
