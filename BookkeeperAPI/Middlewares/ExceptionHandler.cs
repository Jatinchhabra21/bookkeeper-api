﻿namespace BookkeeperAPI.Middlewares
{
    #region usings
    using BookkeeperAPI.Exceptions;
    using BookkeeperAPI.Model;
    using System.Text.Json;
    #endregion

    public class ExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandler> _logger;
        public ExceptionHandler(RequestDelegate next, ILogger<ExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            try
            {
                await _next(context);
            }
            catch (HttpOperationException e)
            {
                context.Response.StatusCode = e.StatusCode;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorResponseModel { ErrorMessage = e.Message, StatusCode = e.StatusCode }, options));
                _logger.LogError(e.Message);
                return;
            }
            catch (Exception e)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(JsonSerializer.Serialize(new ErrorResponseModel { ErrorMessage = e.Message, StatusCode = StatusCodes.Status500InternalServerError }, options));
                _logger.LogError(e, e.Message);
                return;
            }
        }
    }
}
