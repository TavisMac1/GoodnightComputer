using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaToaster;
using GoodnightComputer.Services;
using GoodnightComputer.State;
using GoodnightComputer.ViewModels;
using GoodnightComputer.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GoodnightComputer
{
    public partial class MainWindow : Window
    {
        private bool _isSidebarCollapsed = false;
        private UserControl? _currentView = null;

        public MainWindow()
        {
            InitializeComponent();

            // Set DataContext to LocalizationService for language binding support
            DataContext = LocalizationService.Instance;

            // Subscribe to language changes to refresh the current view
            LocalizationService.Instance.PropertyChanged += OnLocalizationChanged;

            WindowState = WindowState.Maximized;
#if STEAMBUILD
            SystemState.SetSignedIn(false);
#endif

            this._toastNotificationService = Program.Services.GetRequiredService<ToastNotificationService>();
            var toastContainer = this.FindControl<Panel>("ToastContainer");
            this._toastNotificationService.RegisterMainWindow(toastContainer);
#if RELEASEBETA
            SystemState.SignedInChanged += OnSignedInChanged;
# endif
            UpdateMainContent();
        }

        private void NewPlaylistView_Click(object? sender, RoutedEventArgs e) =>
            UpdateMainContent(Program.Services.GetRequiredService<CreatePlaylist>());

        private void NewViewPlaylistsView_Click(object? sender, RoutedEventArgs e) =>
            UpdateMainContent(new ViewPlaylists());

        private void OpenSettings_Click(object? sender, RoutedEventArgs e) =>
            UpdateMainContent(new GoodnightComputer.ViewModels.SettingsViewModel());

        private void OpenPerformanceScan_Click(object? sender, RoutedEventArgs e) =>
            UpdateMainContent(new GoodnightComputer.ViewModels.PerformanceScanViewModel());

        private void OpenSteamStatus_Click(object? sender, RoutedEventArgs e) =>
            UpdateMainContent(new GoodnightComputer.ViewModels.SteamStatusViewModel());

        private ToastNotificationService _toastNotificationService { get; init; }

        private void OpenDiscord_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
#if RELEASEBETA
                if (!SystemState.IsSignedIn)
                {
                    UpdateMainContent();
                    return;
                }
#endif
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
#if RELEASEBETA
            if (!SystemState.IsSignedIn)
            {
                UpdateMainContent();
                return;
            }
#endif

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
                sidebar.Width = 220;
                toggleButton.Content = "◀";
                toggleButton.CornerRadius = new Avalonia.CornerRadius(0, 12, 12, 0);
            }
        }

        private void OnSignedInChanged(bool isSignedIn) => UpdateMainContent();

        private void OnLocalizationChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // When language changes, refresh the current view
            if (e.PropertyName == "CurrentLanguage" || e.PropertyName == string.Empty)
            {
                RefreshCurrentView();
                RefreshNavigationButtons();
                RefreshWindowTitle();
            }
        }

        private void RefreshWindowTitle()
        {
            // Update window title from localization service
            Title = LocalizationService.Instance["AppTitle"];
        }

        private void RefreshNavigationButtons()
        {
            // Update navigation button content directly from localization service
            var loc = LocalizationService.Instance;

            var viewPlaylistsButton = this.FindControl<Button>("ViewPlaylistsButton");
            if (viewPlaylistsButton != null)
                viewPlaylistsButton.Content = loc["Nav_ViewPlaylists"];

            var playlistButton = this.FindControl<Button>("PlaylistButton");
            if (playlistButton != null)
                playlistButton.Content = loc["Nav_CreatePlaylist"];

            var performanceScanButton = this.FindControl<Button>("PerformanceScanButton");
            if (performanceScanButton != null)
                performanceScanButton.Content = loc["Nav_PerformanceScan"];

            var steamStatusButton = this.FindControl<Button>("SteamStatusButton");
            if (steamStatusButton != null)
                steamStatusButton.Content = loc["Nav_Steam"];

            // Update Settings and Discord text (they use nested TextBlocks)
            var settingsText = this.FindControl<TextBlock>("SettingsText");
            if (settingsText != null)
                settingsText.Text = loc["Nav_Settings"];

            var discordText = this.FindControl<TextBlock>("DiscordText");
            if (discordText != null)
                discordText.Text = loc["Nav_Discord"];
        }

        private void RefreshCurrentView()
        {
            if (_currentView == null)
                return;

            // Recreate the current view to pick up new translations
            UserControl? newView = _currentView switch
            {
                ViewPlaylists => new ViewPlaylists(),
                CreatePlaylist => Program.Services.GetRequiredService<CreatePlaylist>(),
                GoodnightComputer.ViewModels.SettingsViewModel => new GoodnightComputer.ViewModels.SettingsViewModel(),
                GoodnightComputer.ViewModels.PerformanceScanViewModel => new GoodnightComputer.ViewModels.PerformanceScanViewModel(),
                GoodnightComputer.ViewModels.SteamStatusViewModel => new GoodnightComputer.ViewModels.SteamStatusViewModel(),
#if RELEASEBETA
                GoodnightComputer.ViewModels.SignInViewModel => Program.Services.GetRequiredService<SignInViewModel>(),
#endif
                _ => _currentView
            };

            if (newView != null)
            {
                UpdateMainContent(newView);
            }
        }

        private void UpdateMainContent(UserControl? requested = null)
        {
            var mainContent = this.FindControl<ContentControl>("MainContent");
            if (mainContent == null)
                return;

#if RELEASEBETA
            if (!SystemState.IsSignedIn)
            {
                var signInView = Program.Services.GetRequiredService<SignInViewModel>();
                mainContent.Content = signInView;
                _currentView = signInView;
                return;
            }
#endif
            var view = requested ?? new ViewPlaylists();
            mainContent.Content = view;
            _currentView = view;
        }
    }
}