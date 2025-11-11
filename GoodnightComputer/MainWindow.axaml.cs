using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using GoodnightComputer.Services;
using GoodnightComputer.ViewModels;
using GoodnightComputer.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;

namespace GoodnightComputer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Initializes MainWindow and its components.
            InitializeComponent();
            var toastService = Program.Services.GetRequiredService<ToastNotificationService>();
            toastService.RegisterMainWindow(this);
        }

        private void NewPlaylistView_Click(object? sender, RoutedEventArgs e)
        {
            this.FindControl<ContentControl>("MainContent").Content = Program.Services.GetRequiredService<CreatePlaylist>();
        }

        private void NewViewPlaylistsView_Click(object? sender, RoutedEventArgs e)
        {
            this.FindControl<ContentControl>("MainContent").Content = new ViewPlaylists();
        }

        private void OpenDiscord_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = Constants.Constants.DISCORD_URL,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                // Optionally handle errors (e.g., show a message to the user)
            }
        }
    }
}