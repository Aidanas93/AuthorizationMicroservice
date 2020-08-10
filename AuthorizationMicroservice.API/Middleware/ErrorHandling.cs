using DnsClient.Internal;
using AuthorizationMicroservice.Application.Infrastructure;
using AuthorizationMicroservice.Application.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace AuthorizationMicroservice.API.Middleware
{
    public class ErrorHandling
    {
        private readonly RequestDelegate next;
        private readonly UnitOfWork unitOfWork;

        public ErrorHandling(RequestDelegate next, UnitOfWork unitOfWork)
        {
            this.next = next;
            this.unitOfWork = unitOfWork;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, unitOfWork.Logger);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex, Microsoft.Extensions.Logging.ILogger logger)
        {
            object errors = null;
            switch (ex)
            {
                case StatusCodeException statusCodeException:
                    logger.LogError(ex, $"HTTP {(int)statusCodeException.Code} {statusCodeException.Error}, with id {statusCodeException.Id}");
                    errors = statusCodeException.Error;
                    context.Response.StatusCode = (int)statusCodeException.Code;
                    break;
                case Exception exception:
                    logger.LogError(ex, exception.Message);
                    errors = string.IsNullOrWhiteSpace(exception.Message) ? "Error" : exception.Message;
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    break;
            }

            context.Response.ContentType = "application/json";
            if (errors != null)
            {
                var result = JsonConvert.SerializeObject(
                    new { errors });
                await context.Response.WriteAsync(result);
            }
        }
    }
}
