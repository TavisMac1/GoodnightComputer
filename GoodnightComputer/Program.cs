using System;
using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using GoodnightComputer.Utils;
using Microsoft.EntityFrameworkCore;

namespace GoodnightComputer
{
    internal class Program
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
                options.UseSqlite("Data Source=app.db"));
            // Register other services, viewmodels, etc. here
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}
