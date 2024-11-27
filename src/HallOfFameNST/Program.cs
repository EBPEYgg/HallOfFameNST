using HallOfFameNST.Middleware;
using HallOfFameNST.Model.Data;
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
        (options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                                         sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()));

    var app = builder.Build();

    app.Logger.LogInformation("Starting the app");

    app.ConfigureExceptionHandler();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<HallOfFameNSTContext>();
        try
        {
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            app.Logger.LogInformation($"Failed to apply migrations: {ex.Message}");
        }
    }

    app.Run();
}
catch (Exception ex)
{
    logger.Error("Stopped program because of exception. ", ex);
    throw;
}
finally
{
    LogManager.Shutdown();
}

public partial class Program { }