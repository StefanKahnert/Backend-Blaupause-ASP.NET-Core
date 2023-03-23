using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Backend_Blaupause.Helper.ExceptionHandling
{
    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;

        public ExceptionHandler(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (HttpException ex)
            {
                await HandleExceptionAsync(context, ex);
            }
            catch (HttpRequestException ex)
            {
                await HandleExceptionAsync(context, new HttpException(ex.StatusCode.Value, ex.Message));
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, HttpException exception)
        {
            HttpStatusCode status;
            string message;

            message = exception.Message;
            status = exception.Status;

            var result = JsonSerializer.Serialize(new { error = message, parameters = exception.Parameters });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            if (exception.Message != null || exception.Parameters != null)
            {
                return context.Response.WriteAsync(result);
            }
            else
            {
                return context.Response.CompleteAsync();
            }
        }
    }
}