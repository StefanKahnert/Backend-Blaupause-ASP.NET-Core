using Backend_Blaupause.Models;
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
    [Route("[controller]")]
    public class HeadController : ControllerBase
    {
        [HttpHead]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        public async Task<IActionResult> head()
        {
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        [HttpGet("{input}")]
        [ResponseCache(Duration = 10000, VaryByQueryKeys = new[] { "*" })]
        public async Task<ActionResult<string>> GetCacheResult(string input)
        {
            var now = DateTime.Now;
            return $"Hallo, dieser Response wurde um {now.ToString()} erstellt.";
        }
    }
}
