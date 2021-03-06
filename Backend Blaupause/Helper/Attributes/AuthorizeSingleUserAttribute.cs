using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Backend_Blaupause.Helper;
using Backend_Blaupause.Helper.ExceptionHandling;

namespace Backend_Blaupause.Helper
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class AuthorizeSingleUser : TypeFilterAttribute
    {

        public AuthorizeSingleUser() : base(typeof(AuthorizeSingleUserFilter))
        {

        }


        private class AuthorizeSingleUserFilter : IAsyncResultFilter
        {
            private readonly UserAuthentication userAuthentication;

            public AuthorizeSingleUserFilter(UserAuthentication userAuthentication)
            {
                this.userAuthentication = userAuthentication;
            }

            public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
            {
                try
                {
                    await userAuthentication.checkToken(context.HttpContext.Request.Headers["Authorization"]);
                }
                catch
                {
                    throw new HttpException(HttpStatusCode.Unauthorized, "");
                }
            }
        }

    }
}
