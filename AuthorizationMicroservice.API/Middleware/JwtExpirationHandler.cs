using AuthorizationMicroservice.Application.Infrastructure;
using AuthorizationMicroservice.Application.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Net;
using System.Threading.Tasks;

namespace AuthorizationMicroservice.API.Middleware
{
    public class JwtExpirationHandler
    {
        private readonly RequestDelegate next;
        private readonly UnitOfWork unitOfWork;

        public JwtExpirationHandler(RequestDelegate next, UnitOfWork unitOfWork)
        {
            this.next = next;
            this.unitOfWork = unitOfWork;
        }

        public async Task Invoke(HttpContext context)
        {
            StringValues strings;
            context.Request.Headers.TryGetValue("Authorization", out strings);
            var jwtToken = strings.ToString();

            if (!String.IsNullOrEmpty(jwtToken))
            {
                if (!unitOfWork.JWTHandler.IsValid(unitOfWork.JWTHandler.RemoveBearerFromJWTToken(jwtToken)))
                {
                    throw new StatusCodeException(HttpStatusCode.RequestTimeout, "Not valid token");
                }
            }


            await next(context);
        }

    }
}
