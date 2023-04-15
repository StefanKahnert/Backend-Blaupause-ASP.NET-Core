using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Backend_Blaupause.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class CacheController : ControllerBase
    {

        private readonly IMemoryCache _memoryCache;

        public CacheController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        /// <summary>
        /// Cache Response Example
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("{input}")]
        [ResponseCache(Duration = 10000, VaryByQueryKeys = new[] { "*" })]
        public async Task<ActionResult<string>> GetCacheResult(string input)
        {
            var now = DateTime.Now.TimeOfDay;
            return $"Hallo {input}, dieser Response wurde um {now} erstellt.";
        }

        /// <summary>
        /// Cache Response by input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet("memory/{input}")]
        public async Task<ActionResult<string>> GetCacheResultMemoryCache(string input)
        {
            return Ok(await _memoryCache.GetOrCreateAsync(input, async cacheEntry =>
            {
                var now = DateTime.Now.TimeOfDay;
                return $"Hallo {input}, dieser Response wurde um {now} erstellt.";
            }));

        }

        /// <summary>
        /// Clear Cache
        /// </summary>
        /// <returns></returns>
        [HttpPut("memory/clear")]
        public async Task<ActionResult<string>> InvalidateCache()
        {
            PropertyInfo prop = _memoryCache.GetType().GetProperty("EntriesCollection", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Public);
            var secretCache = prop?.GetValue(_memoryCache);
            MethodInfo clearMethod = secretCache?.GetType().GetMethod("Clear", BindingFlags.Instance | BindingFlags.Public);
            clearMethod?.Invoke(secretCache, null);

            //Ab .NET 7: _memoryCache.Clear();

            return Ok("Cache has been cleared");

        }
    }
}
