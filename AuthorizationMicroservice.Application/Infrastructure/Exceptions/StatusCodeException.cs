using System;
using System.Net;

namespace AuthorizationMicroservice.Application.Infrastructure.Exceptions
{
    public class StatusCodeException : Exception
    {
        public StatusCodeException(HttpStatusCode code, string error)
        {
            Code = code;
            Error = error;
        }

        public StatusCodeException(HttpStatusCode code, string error, Guid id)
        {
            Code = code;
            Error = error;
            Id = id;
        }

        public HttpStatusCode Code { get; }
        public string Error { get; }
        public Guid Id { get; } = Guid.Empty;
    }
}
