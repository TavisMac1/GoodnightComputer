using Avalonia;
using AvaloniaToaster;
using GoodnightComputer.Models;
using GoodnightComputer.State;
using GoodnightComputer.Utils;
using GoodnightComputer.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace GoodnightComputer;

public class Program
{
    public static IServiceProvider Services { get; private set; } = null!;

    [STAThread]
    public static void Main(string[] args)
    {
        // Configure DI
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        Services = serviceCollection.BuildServiceProvider();

        // Run async initialization synchronously
        SystemState.STATIC_APPLICATION_NAMES = Task.Run(GeneralUtils.ListApplicationNames).Result;

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={Constants.Constants.DB_NAME}")
        );

        services.AddSingleton<ToastNotificationService>();
        services.AddSingleton<PlaylistState>();
        services.AddTransient<CreatePlaylist>();
        services.AddTransient<ViewPlaylistsViewModel>();
        services.AddSingleton<ToastNotificationService>();
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
