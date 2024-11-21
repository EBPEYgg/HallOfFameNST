using HallOfFameNST.Model.Data;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("Initializing application");

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddDbContext<HallOfFameNSTContext>
        (options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    var app = builder.Build();

    app.Logger.LogInformation("Starting the app");

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

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    logger.Error("Stopped program because of exception. ", ex);
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}