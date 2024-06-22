using Octagram.API.Utilities;
using Octagram.Infrastructure.Data.Context;

namespace Octagram.API;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = CreateHostBuilder(args).Build();
        

        // Seed data to the database for testing purposes
        using (var serviceScope = builder.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
            var seeder = new DataSeeder(context!);
            await seeder.SeedAsync();
        }

        await builder.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
}