using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartUserIntegration.ApiClient.Interfaces;
using SmartUserIntegration.ApiClient.Services;
using SmartUserIntegration.ApiClient.Configuration;

namespace SmartUserIntegration.ApiClient.Extensions;

public static class HttpClientExtensions
{
    public static IServiceCollection AddExternalUserService(this IServiceCollection services, IConfiguration config)
    {
        var apiSettings = config.GetSection("ApiClient").Get<ApiSettings>();

        if (string.IsNullOrWhiteSpace(apiSettings?.BaseUrl))
            throw new InvalidOperationException("BaseUrl is missing in config.");

        Console.WriteLine("DEBUG BaseUrl: " + apiSettings.BaseUrl); // Debug log

        services.AddHttpClient<IExternalUserService, ExternalUserService>(client =>
        {
            client.BaseAddress = new Uri(apiSettings.BaseUrl!);
            client.DefaultRequestHeaders.Add("x-api-key", apiSettings.ApiKey);
        });

        return services;
    }
}
