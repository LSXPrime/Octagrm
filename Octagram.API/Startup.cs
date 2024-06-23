using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Octagram.API.Filters;
using Octagram.API.Hubs;
using Octagram.API.Utilities;
using Octagram.Infrastructure.Data.Context;

namespace Octagram.API;

public class Startup(IConfiguration configuration)
{
    private IConfiguration Configuration { get; } = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        // 1. Configure Services
        services.AddControllers();
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        services.AddMvc(options => options.Filters.Add(typeof(ModelStateValidationFilter)));
        
        // 2. Configure AutoMapper
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


        // 3. Register Repositories
        services.AddRepositories();

        // 4. Register Services
        services.AddServices();
        
        // 5. Register SignalR
        services.AddSignalR();

        // 6. Configure Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection()
            .UseRouting()
            .UseAuthentication()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<DirectMessageHub>("/directmessageshub");
                endpoints.MapHub<NotificationHub>("/notificationhub");
            })
            .UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        var error = new { message = contextFeature.Error.Message };
                        await context.Response.WriteAsync(JsonSerializer.Serialize(error));

                        // Logging
                        Console.WriteLine($"Error: {contextFeature.Error}");
                        Console.WriteLine($"Stack Trace: {contextFeature.Error.StackTrace}");
                    }
                });
            });
    }
}