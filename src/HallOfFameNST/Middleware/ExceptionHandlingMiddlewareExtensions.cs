using Microsoft.AspNetCore.Diagnostics;

namespace HallOfFameNST.Middleware
{
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this WebApplication app)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    if (exceptionHandlerPathFeature?.Error is not null)
                    {
                        logger.LogError(exceptionHandlerPathFeature.Error, "Unhandled exception occurred.");

                        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
                        {
                            error = "An unexpected error occurred. Please try again later."
                        }));
                    }
                });
            });
        }
    }
}