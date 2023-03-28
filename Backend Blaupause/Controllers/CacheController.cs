using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Backend_Blaupause.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CacheController : ControllerBase
    {

        [HttpGet("{input}")]
        [ResponseCache(Duration = 10000, VaryByQueryKeys = new[] { "*" })]
        public async Task<ActionResult<string>> GetCacheResult(string input)
        {
            var now = DateTime.Now.TimeOfDay;
            return $"Hallo {input}, dieser Response wurde um {now} erstellt.";
        }
    }
}
