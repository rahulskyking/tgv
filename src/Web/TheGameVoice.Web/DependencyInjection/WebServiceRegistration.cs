using Microsoft.Extensions.DependencyInjection;
using TheGameVoice.Web.Services;

namespace TheGameVoice.Web.DependencyInjection;

public static class WebServiceRegistration
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddControllersWithViews();
        services.AddHttpContextAccessor();

        // Site mode is resolved per request from the query string or cookie.
        services.AddScoped<ISiteSegmentAccessor, SiteSegmentAccessor>();

        // Holds only the configured public origin; no request-specific state.
        services.AddSingleton<PublicSiteUrl>();

        return services;
    }
}
