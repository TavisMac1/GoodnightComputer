using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using GoodnightComputer.Models;
using GoodnightComputer.State;
using GoodnightComputer.Utils;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace GoodnightComputer.ViewModels;

public partial class SteamStatusViewModel : UserControl
{
    public ObservableCollection<PlaylistRecord> Playlists { get; } = new();

    private Timer? _timer;
    private bool _wasGameRunning;
    private bool _wasDownloading;

    public SteamStatusViewModel()
    {
        InitializeComponent();
        DataContext = this;

        foreach (var p in PlaylistState.GlobalPlaylistRecords)
            Playlists.Add(p);

        Refresh();
        _timer = new Timer(_ => Dispatcher.UIThread.Post(Refresh), null,
            TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
    }

    private void Refresh()
    {
        SteamStatus s;
        try   { s = SteamUtils.GetStatus(); }
        catch { s = new SteamStatus { IsSteamInstalled = false }; }

        UpdateView(s);
        HandleTriggers(s);

        _wasGameRunning = s.IsGameRunning;
        _wasDownloading = s.IsDownloading;
    }

    // ── Trigger logic ────────────────────────────────────────────────────────

    private void HandleTriggers(SteamStatus s)
    {
        // Game running → activate playlist on start, deactivate on stop
        HandleTrigger(
            isActive:    s.IsGameRunning,
            wasActive:   _wasGameRunning,
            playlistId:  SteamTriggerState.GameRunningPlaylistId);

        // Downloading → activate playlist on start, deactivate on stop
        HandleTrigger(
            isActive:    s.IsDownloading,
            wasActive:   _wasDownloading,
            playlistId:  SteamTriggerState.DownloadingPlaylistId);
    }

    private static void HandleTrigger(bool isActive, bool wasActive, Guid? playlistId)
    {
        if (playlistId == null) return;
        var playlist = PlaylistState.GlobalPlaylistRecords.FirstOrDefault(p => p.Id == playlistId);
        if (playlist == null) return;

        if (isActive && !wasActive && !playlist.IsActive)
            PlaylistState.SleepPlaylistApplications(playlist);
        else if (!isActive && wasActive && playlist.IsActive)
            PlaylistState.ResumePlaylistApplications(playlist);
    }

    // ── Trigger assignment buttons ───────────────────────────────────────────

    private void SetGameTrigger_Click(object? sender, RoutedEventArgs e)
    {
        var combo = this.FindControl<ComboBox>("GamePlaylistCombo");
        SteamTriggerState.GameRunningPlaylistId = (combo?.SelectedItem as PlaylistRecord)?.Id;
        UpdateTriggerLabels();
    }

    private void ClearGameTrigger_Click(object? sender, RoutedEventArgs e)
    {
        SteamTriggerState.GameRunningPlaylistId = null;
        if (this.FindControl<ComboBox>("GamePlaylistCombo") is { } c) c.SelectedItem = null;
        UpdateTriggerLabels();
    }

    private void SetDownloadTrigger_Click(object? sender, RoutedEventArgs e)
    {
        var combo = this.FindControl<ComboBox>("DownloadPlaylistCombo");
        SteamTriggerState.DownloadingPlaylistId = (combo?.SelectedItem as PlaylistRecord)?.Id;
        UpdateTriggerLabels();
    }

    private void ClearDownloadTrigger_Click(object? sender, RoutedEventArgs e)
    {
        SteamTriggerState.DownloadingPlaylistId = null;
        if (this.FindControl<ComboBox>("DownloadPlaylistCombo") is { } c) c.SelectedItem = null;
        UpdateTriggerLabels();
    }

    // ── View helpers ─────────────────────────────────────────────────────────

    private void UpdateView(SteamStatus s)
    {
        SetVisible("NotInstalledPanel", !s.IsSteamInstalled);
        SetVisible("StatusPanel", s.IsSteamInstalled);
        if (!s.IsSteamInstalled) return;

        SetVisible("GameRunningPanel", s.IsGameRunning);
        SetText("RunningGameText", s.RunningGameName ?? string.Empty);

        SetVisible("DownloadingPanel", s.IsDownloading);
        SetText("DownloadingGameText", s.DownloadingGameName ?? string.Empty);

        if (this.FindControl<ProgressBar>("DownloadBar") is { } bar && s.TotalBytes > 0)
        {
            bar.Maximum = s.TotalBytes;
            bar.Value   = s.DownloadedBytes;
        }

        SetVisible("IdlePanel", !s.IsGameRunning && !s.IsDownloading);
        UpdateTriggerLabels();
    }

    private void UpdateTriggerLabels()
    {
        var gameName = SteamTriggerState.GameRunningPlaylistId is { } gid
            ? PlaylistState.GlobalPlaylistRecords.FirstOrDefault(p => p.Id == gid)?.Name ?? "Unknown"
            : "None";
        SetText("GameTriggerLabel", $"Active trigger: {gameName}");

        var dlName = SteamTriggerState.DownloadingPlaylistId is { } did
            ? PlaylistState.GlobalPlaylistRecords.FirstOrDefault(p => p.Id == did)?.Name ?? "Unknown"
            : "None";
        SetText("DownloadTriggerLabel", $"Active trigger: {dlName}");
    }

    private void SetText(string name, string text)
    {
        if (this.FindControl<TextBlock>(name) is { } tb) tb.Text = text;
    }

    private void SetVisible(string name, bool visible)
    {
        if (this.FindControl<Control>(name) is { } c) c.IsVisible = visible;
    }
}
