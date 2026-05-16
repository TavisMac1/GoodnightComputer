using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GoodnightComputer.Models;
using System.Linq;

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
                desktop.MainWindow = new MainWindow();
                desktop.ShutdownRequested += OnShutdownRequested;
            }

            base.OnFrameworkInitializationCompleted();
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
