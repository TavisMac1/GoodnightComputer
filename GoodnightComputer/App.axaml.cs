using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GoodnightComputer.Models;
using GoodnightComputer.Views;
using AvaloniaToaster;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace GoodnightComputer
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.ShutdownRequested += OnShutdownRequested;

                var splash = new SplashScreen();
                desktop.MainWindow = splash;

                splash.AnimationComplete += () =>
                {
                    var mainWindow = new MainWindow();
                    desktop.MainWindow = mainWindow;
                    mainWindow.Show();
                    SetupTrayIcon(desktop, mainWindow);
                };

                splash.Show();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private TrayIcon? _trayIcon;

        private void SetupTrayIcon(IClassicDesktopStyleApplicationLifetime desktop, MainWindow mainWindow)
        {
            void Restore()
            {
                mainWindow.Show();
                if (mainWindow.WindowState == WindowState.Minimized)
                    mainWindow.WindowState = WindowState.Maximized;
                mainWindow.Activate();
            }

            var openItem = new NativeMenuItem("Open");
            openItem.Click += (_, _) => Restore();
            var exitItem = new NativeMenuItem("Exit");
            exitItem.Click += (_, _) => desktop.Shutdown();

            var menu = new NativeMenu();
            menu.Items.Add(openItem);
            menu.Items.Add(exitItem);

            _trayIcon = new TrayIcon
            {
                Icon = mainWindow.Icon,
                ToolTipText = "Goodnight Computer",
                Menu = menu,
                IsVisible = true
            };
            _trayIcon.Clicked += (_, _) => Restore();

            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            var hiding = false;

            async void HideToTray()
            {
                if (hiding || !mainWindow.IsVisible)
                    return;
                hiding = true;
                try
                {
                    if (mainWindow.WindowState != WindowState.Minimized)
                    {
                        Program.Services.GetService<ToastNotificationService>()
                            ?.Show("Goodnight Computer is still running in the system tray.", 2500);
                        await Task.Delay(2600);
                    }
                    mainWindow.Hide();
                }
                finally
                {
                    hiding = false;
                }
            }

            mainWindow.Closing += (_, e) =>
            {
                if (e.CloseReason == WindowCloseReason.WindowClosing)
                {
                    e.Cancel = true;
                    HideToTray();
                }
            };

            mainWindow.PropertyChanged += (_, e) =>
            {
                if (e.Property == Window.WindowStateProperty && mainWindow.WindowState == WindowState.Minimized)
                    HideToTray();
            };
        }

        private void OnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
        {
            var activePlaylists = PlaylistState.GlobalPlaylistRecords
                .Where(p => p.IsActive)
                .ToList();

            foreach (var playlist in activePlaylists)
            {
                try
                {
                    PlaylistState.ResumePlaylistApplications(playlist);
                }
                catch
                {
                    // Best-effort — don't block shutdown if a resume fails
                }
            }
        }
    }
}
