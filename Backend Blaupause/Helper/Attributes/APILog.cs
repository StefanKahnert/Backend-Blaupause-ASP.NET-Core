using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Backend_Blaupause.Models.Interfaces;
using System.Net;
using Backend_Blaupause.Helper.ExceptionHandling;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;


namespace Backend_Blaupause.Helper
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class APILog : TypeFilterAttribute
    {

        public APILog([CallerMemberName] string caller = null) : base(typeof(APILogFilter))
        {
            Arguments = new object[] { caller };
        }


        private class APILogFilter : IResultFilter
        {

            //private readonly ILogger logger;
            private readonly string caller;
            private readonly ILogger<APILogFilter> logger;
            private readonly UserAuthentication userAuthentication;

            public APILogFilter(UserAuthentication userAuthentication, ILogger<APILogFilter> logger, string caller)
            {
                this.caller = caller;
                this.logger = logger;
                this.userAuthentication = userAuthentication;
            }

            public void OnResultExecuted(ResultExecutedContext context)
            {
            }

            public void OnResultExecuting(ResultExecutingContext context)
            {
                logger.LogInformation(userAuthentication.GetUserId() == -1 ? "Unknown User" : userAuthentication.GetUserId() + " has called API: " + caller + " in Controller " + context.Controller.GetType());  
            }
        }

    }
}
