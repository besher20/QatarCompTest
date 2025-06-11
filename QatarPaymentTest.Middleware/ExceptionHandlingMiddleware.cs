
using System.Net;
using System.Text.Json;

using Microsoft.AspNetCore.Http;
using Serilog.Core;

namespace QatarPaymentTest.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
         
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // Proceed to the next middleware
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = HttpStatusCode.InternalServerError;

            var response = new
            {
                StatusCode = (int)statusCode,
                Message = "An unexpected error occurred. Please try again later.",
                Details = exception.Message // Optional: remove in production
            };

            var payload = JsonSerializer.Serialize(response);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(payload);
        }
    }

}
