using System.Collections.Generic;
using System;
using System.Net;

namespace Backend_Blaupause.Helper.ExceptionHandling
{
    public class HttpException : Exception
    {
        public HttpException(HttpStatusCode statusCode, string message) : base(message)
        {
            Status = statusCode;
            Message = message;
        }

        public HttpException(HttpStatusCode statusCode) : base(null)
        {
            Status = statusCode;
            Message = String.Empty;
        }

        public HttpException(HttpStatusCode statusCode, string message, params (string, object)[] parameters) : base(message)
        {
            Status = statusCode;
            Message = message;
            Parameters = new Dictionary<string, object>();
            foreach (var parameter in parameters)
            {
                Parameters.Add(parameter.Item1, parameter.Item2);
            }
        }

        public override string Message { get; }
        public HttpStatusCode Status { get; set; }
        public Dictionary<string, object> Parameters { get; set; }

    }
}