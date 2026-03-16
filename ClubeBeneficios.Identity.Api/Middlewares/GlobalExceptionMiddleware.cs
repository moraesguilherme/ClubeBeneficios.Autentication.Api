using System.Net;
using System.Text.Json;
using FluentValidation;
using ClubeBeneficios.Identity.Domain.Models.DTOs;

namespace ClubeBeneficios.Identity.Api.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await WriteResponseAsync(
                context,
                HttpStatusCode.BadRequest,
                "Erro de validação.",
                string.Join(" | ", ex.Errors.Select(e => e.ErrorMessage)));
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteResponseAsync(context, HttpStatusCode.Unauthorized, ex.Message, null);
        }
        catch (ArgumentException ex)
        {
            await WriteResponseAsync(context, HttpStatusCode.BadRequest, ex.Message, null);
        }
        catch (Exception ex)
        {
            await WriteResponseAsync(context, HttpStatusCode.InternalServerError, "Ocorreu um erro interno.", ex.Message);
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, string message, string? detail)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new ApiErrorResponseDto
        {
            Message = message,
            Detail = detail,
            StatusCode = (int)statusCode
        };

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}
