using Octagram.API.Hubs;
using Octagram.Application.Interfaces;
using Octagram.Application.Services;
using Octagram.Domain.Repositories;
using Octagram.Infrastructure.Repositories;
using Octagram.Infrastructure.Utilities;

namespace Octagram.API.Utilities;

public static class ServiceExtensions
{
    /// <summary>
    /// Adds repositories to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add repositories to.</param>
    /// <returns>The service collection with added repositories.</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<ILikeRepository, LikeRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IFollowRepository, FollowRepository>();
        services.AddScoped<IStoryRepository, StoryRepository>();
        services.AddScoped<IHashtagRepository, HashtagRepository>();
        services.AddScoped<IPostHashtagRepository, PostHashtagRepository>();
        services.AddScoped<IDirectMessageRepository, DirectMessageRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        
        return services;
    }
    
    /// <summary>
    /// Adds services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <returns>The service collection with added services.</returns>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPostService, PostService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISearchService, SearchService>();
        services.AddScoped<IStoryService, StoryService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IDirectMessageService, DirectMessageService>();
        services.AddScoped<IImageHelper, ImageHelper>();
        services.AddScoped<ICloudStorageHelper, CloudStorageHelper>();
        services.AddScoped<INotificationPublisher, NotificationPublisher>();
        
        return services;
    }
}