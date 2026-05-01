using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TaskFlow2.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (KeyNotFoundException ex)
            {
                await WriteErrorAsync(context, HttpStatusCode.NotFound, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                await WriteErrorAsync(context, HttpStatusCode.Forbidden, ex.Message);
            }
            catch (ArgumentException ex)
            {
                await WriteErrorAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception ex)
            {
                await WriteErrorAsync(context, HttpStatusCode.InternalServerError,
                    "Une erreur interne est survenue : " + ex.Message);
            }
        }

        private static async Task WriteErrorAsync(HttpContext context, HttpStatusCode code, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            var response = JsonSerializer.Serialize(new
            {
                statusCode = (int)code,
                message
            });

            await context.Response.WriteAsync(response);
        }
    }
}