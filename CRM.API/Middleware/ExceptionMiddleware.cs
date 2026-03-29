using FluentValidation;
using CRM.Application.Common.Exceptions;


namespace CRM.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";

                context.Response.StatusCode = ex switch
                {
                    ValidationException => 400,
                    UnauthorizedException => 401,
                    ConflictException => 409,
                    _ => 500
                };

                var response = new
                {
                    message = ex.Message
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
