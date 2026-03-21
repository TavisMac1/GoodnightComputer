using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaToaster;
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
            InitializeComponent();
            
            WindowState = WindowState.Maximized;
            
            this._toastNotificationService = Program.Services.GetRequiredService<ToastNotificationService>();
            var toastContainer = this.FindControl<Panel>("ToastContainer");
            this._toastNotificationService.RegisterMainWindow(toastContainer);
            this.FindControl<ContentControl>("MainContent")!.Content = new ViewPlaylists();
        }

        private void NewPlaylistView_Click(object? sender, RoutedEventArgs e) =>
            this.FindControl<ContentControl>("MainContent")!.Content = Program.Services.GetRequiredService<CreatePlaylist>();

        private void NewViewPlaylistsView_Click(object? sender, RoutedEventArgs e) =>
            this.FindControl<ContentControl>("MainContent")!.Content = new ViewPlaylists();

        private ToastNotificationService _toastNotificationService { get; init; }

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
                this._toastNotificationService.Show(ex.Message);
            }
        }
    }
}