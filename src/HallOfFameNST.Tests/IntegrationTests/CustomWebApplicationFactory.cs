using HallOfFameNST.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HallOfFameNST.Tests.IntegrationTests
{
    /// <inheritdoc/>
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll(typeof(DbContextOptions<HallOfFameNSTContext>));
                services.AddEntityFrameworkInMemoryDatabase()
                        .AddDbContext<HallOfFameNSTContext>((sp, options) =>
                {
                    options.UseInMemoryDatabase("HallOfFameTestDb").UseInternalServiceProvider(sp);
                });
            });
        }
    }
}