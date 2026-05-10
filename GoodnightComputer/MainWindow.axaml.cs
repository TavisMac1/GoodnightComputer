using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaToaster;
using GoodnightComputer.State;
using GoodnightComputer.ViewModels;
using GoodnightComputer.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;

namespace GoodnightComputer
{
    public partial class MainWindow : Window
    {
        private bool _isSidebarCollapsed = false;

        public MainWindow()
        {
            InitializeComponent();
            
            WindowState = WindowState.Maximized;
            SystemState.SetSignedIn(false);
            
            this._toastNotificationService = Program.Services.GetRequiredService<ToastNotificationService>();
            var toastContainer = this.FindControl<Panel>("ToastContainer");
            this._toastNotificationService.RegisterMainWindow(toastContainer);
            SystemState.SignedInChanged += OnSignedInChanged;

            UpdateMainContent();
        }

        private void NewPlaylistView_Click(object? sender, RoutedEventArgs e) =>
            UpdateMainContent(Program.Services.GetRequiredService<CreatePlaylist>());

        private void NewViewPlaylistsView_Click(object? sender, RoutedEventArgs e) =>
            UpdateMainContent(new ViewPlaylists());

        private ToastNotificationService _toastNotificationService { get; init; }

        private void OpenDiscord_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (!SystemState.IsSignedIn)
                {
                    UpdateMainContent();
                    return;
                }

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

        private void ToggleSidebar_Click(object? sender, RoutedEventArgs e)
        {
            if (!SystemState.IsSignedIn)
            {
                UpdateMainContent();
                return;
            }

            var sidebar = this.FindControl<Border>("Sidebar");
            var toggleButton = this.FindControl<Button>("ToggleSidebarButton");

            if (sidebar == null || toggleButton == null)
                return;

            _isSidebarCollapsed = !_isSidebarCollapsed;

            if (_isSidebarCollapsed)
            {
                // Collapse sidebar
                sidebar.Width = 10;
                toggleButton.Content = "▶";
                toggleButton.CornerRadius = new Avalonia.CornerRadius(12, 0, 0, 12);
            }
            else
            {
                // Expand sidebar
                sidebar.Width = 180;
                toggleButton.Content = "◀";
                toggleButton.CornerRadius = new Avalonia.CornerRadius(0, 12, 12, 0);
            }
        }

        private void OnSignedInChanged(bool isSignedIn) => UpdateMainContent();

        private void UpdateMainContent(UserControl? requested = null)
        {
            var mainContent = this.FindControl<ContentControl>("MainContent");
            if (mainContent == null)
                return;

            if (!SystemState.IsSignedIn)
            {
                mainContent.Content = Program.Services.GetRequiredService<SignInViewModel>();
                return;
            }

            mainContent.Content = requested ?? new ViewPlaylists();
        }
    }
}