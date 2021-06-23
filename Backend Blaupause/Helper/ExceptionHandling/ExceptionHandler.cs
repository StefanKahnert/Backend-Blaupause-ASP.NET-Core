using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Backend_Blaupause.Helper.ExceptionHandling
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;
        public readonly ILogger<ExceptionHandler> logger;

        public ExceptionHandler(RequestDelegate next, ILogger<ExceptionHandler> logger)
        {
            _next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, logger);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception, ILogger<ExceptionHandler> logger)
        {
            HttpStatusCode status;
            string message;
            var stackTrace = String.Empty;

            var exceptionType = exception.GetType();

            if (exceptionType == typeof(HttpException))
            {
                HttpException httpException = (HttpException) exception;
                message = exception.Message;
                status = httpException.status;
            }
            else
            {
                status = HttpStatusCode.InternalServerError;
                message = exception.Message;
                logger.LogError(exception.Message + "\r\n" + exception.StackTrace);

            }

            var result = JsonSerializer.Serialize(new { error = message, stackTrace });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            return context.Response.WriteAsync(result);
        }
    }
}

