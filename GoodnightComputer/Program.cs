using Avalonia;
using GoodnightComputer.Models;
using GoodnightComputer.Services;
using GoodnightComputer.Utils;
using GoodnightComputer.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GoodnightComputer;

public class Program
{
    public static IServiceProvider Services { get; private set; }

    [STAThread]
    public static void Main(string[] args)
    {
        // Configure DI
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);
        Services = serviceCollection.BuildServiceProvider();

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
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
