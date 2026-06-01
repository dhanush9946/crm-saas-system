using System.Net;
using System.Text.Json;
using FluentValidation;
using CRM.API.Responses;
using CRM.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context); // go to next middleware / controller
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var traceId = context.TraceIdentifier;

            context.Response.ContentType = "application/json";

            var response = new ErrorResponse
            {
                TraceId = traceId
            };

            switch (ex)
            {
                case ValidationException validationException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.ErrorCode = "VALIDATION_ERROR";
                    response.Message = "Validation failed";
                    response.Details = validationException.Errors
                        .Select(e => new { e.PropertyName, e.ErrorMessage });
                    break;

                case UnauthorizedException:
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    response.ErrorCode = "UNAUTHORIZED";
                    response.Message = ex.Message;
                    break;

                case ForbiddenException:
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    response.ErrorCode = "FORBIDDEN";
                    response.Message = ex.Message;
                    break;

                case BadRequestException:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.ErrorCode = "BAD_REQUEST";
                    response.Message = ex.Message;
                    break;

                case ConflictException:
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    response.ErrorCode = "CONFLICT";
                    response.Message = ex.Message;
                    break;

                case NotFoundException:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.ErrorCode = "NOT_FOUND";
                    response.Message = ex.Message;
                    break;

                case DbUpdateConcurrencyException: // Handles EF Core's concurrency exception
                case ConcurrencyException:         // Handles your custom application exception
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    response.ErrorCode = "CONCURRENCY_CONFLICT";
                    response.Message = "The record has been modified or deleted by another user since it was loaded. Please reload and try again.";
                    break;



                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    response.ErrorCode = "INTERNAL_SERVER_ERROR";
                    response.Message = "Something went wrong";
                    break;
            }

            var json = JsonSerializer.Serialize(response, JsonOptions);

            await context.Response.WriteAsync(json);
        }
    }
}
