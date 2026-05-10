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
        var dbPath = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "GoodnightComputer",
            Constants.Constants.DB_NAME);
        System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dbPath)!);
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}")
        );

        services.AddSingleton<ToastNotificationService>();
        services.AddTransient<CreatePlaylist>();
        services.AddTransient<ViewPlaylistsViewModel>();
        services.AddTransient<SignInViewModel>();
        services.AddSingleton<ToastNotificationService>();
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
