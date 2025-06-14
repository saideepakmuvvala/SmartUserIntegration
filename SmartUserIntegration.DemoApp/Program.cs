using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmartUserIntegration.ApiClient.Extensions;
using SmartUserIntegration.ApiClient.Interfaces;

var builder = Host.CreateDefaultBuilder()
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        var env = hostingContext.HostingEnvironment;
        config.SetBasePath(AppContext.BaseDirectory);
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((ctx, services) =>
    {
        services.AddExternalUserService(ctx.Configuration);
    });

var host = builder.Build();
var userService = host.Services.GetRequiredService<IExternalUserService>();

var config = host.Services.GetRequiredService<IConfiguration>();
Console.WriteLine("DEBUG BaseUrl: " + config["ApiClient:BaseUrl"]);
Console.WriteLine("DEBUG API Key: " + config["ApiClient:ApiKey"]);

var allUsers = await userService.GetAllUsersAsync();

foreach (var user in allUsers)
{
    Console.WriteLine($"{user.Id}: {user.FullName} - {user.Email}");
}
