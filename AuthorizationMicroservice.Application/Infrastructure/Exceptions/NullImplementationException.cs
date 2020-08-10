using System;

namespace AuthorizationMicroservice.Application.Infrastructure.Exceptions
{
    public class NullImplementationException : Exception
    {
        public NullImplementationException(string message) : base(message)
        {

        }
    }
}
