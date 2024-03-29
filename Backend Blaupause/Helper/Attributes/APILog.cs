﻿using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;

namespace Backend_Blaupause.Helper
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class APILog : TypeFilterAttribute
    {

        public APILog([CallerMemberName] string caller = null) : base(typeof(APILogFilter))
        {
            Arguments = new object[] { caller };
        }


        private class APILogFilter : IAsyncResultFilter
        {

            //private readonly ILogger logger;
            private readonly string _caller;
            private readonly ILogger<APILogFilter> _logger;

            public APILogFilter(ILogger<APILogFilter> logger, string caller)
            {
                _caller = caller;
                _logger = logger;
            }

            public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
            {
                var stream = (await context.HttpContext.GetTokenAsync("access_token"));
                string userId;

                if(stream == null)
                {
                    userId = null;
                } else
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jsonToken = handler.ReadToken(stream);
                    var token = jsonToken as JwtSecurityToken;

                    userId = token.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub)?.Value;
                }


                _logger.LogInformation((string.IsNullOrWhiteSpace(userId) ? "Unknown User" : ("UserId: " + userId)) + " has called API: " + _caller + " in Controller " + context.Controller.GetType());

                await next.Invoke();
            }
        }

    }
}
