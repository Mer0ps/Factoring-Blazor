using Domain.Models;
using System.Net;

namespace Mx.Blazor.DApp.Client.App.Exceptions
{
    public class HttpException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Error { get; set; }

        public HttpException(HttpResponse httpResponse) : base(httpResponse.Message)
        {
            StatusCode = httpResponse.StatusCode;
            Error = httpResponse.Error;
        }

    }
}
