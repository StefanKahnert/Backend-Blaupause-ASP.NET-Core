using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Backend_Blaupause.Helper.ExceptionHandling
{
    public class HttpException : Exception
    {
        public HttpStatusCode status { get; set; }
        public string message { get; set; }
        public HttpException(HttpStatusCode statusCode, string message) : base(message)
        {
            this.status = statusCode;
            this.message = message;
        }
    }
}
