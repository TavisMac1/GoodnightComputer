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
using System.Threading.Tasks;

namespace GoodnightComputer.ViewModels;

public partial class PerformanceScanViewModel : UserControl
{
    public ObservableCollection<ApplicationRecord> Results { get; } = new();

    private CancellationTokenSource? _cts;

    public PerformanceScanViewModel()
    {
        InitializeComponent();
        DataContext = this;
    }

    private async void StartScan_Click(object? sender, RoutedEventArgs e)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        Results.Clear();
        SetStatus("Starting scan…");
        SetScanningState(true);

        try
        {
            var progress = new Progress<string>(msg => Dispatcher.UIThread.Post(() => SetStatus(msg)));
            var records = await PerformanceScanner.ScanAsync(
                durationSeconds: 5,
                intervalMs: 500,
                progress: progress,
                cancellationToken: _cts.Token);

            Dispatcher.UIThread.Post(() =>
            {
                Results.Clear();
                foreach (var r in records)
                    Results.Add(r);

                SetStatus($"Scan complete — {records.Count} apps measured");
                SetScanningState(false);
            });
        }
        catch (OperationCanceledException)
        {
            SetStatus("Scan cancelled");
            SetScanningState(false);
        }
        catch (Exception ex)
        {
            SetStatus($"Error: {ex.Message}");
            SetScanningState(false);
        }
    }

    private void SetStatus(string text)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var tb = this.FindControl<TextBlock>("StatusText");
            if (tb != null) tb.Text = text;
        });
    }

    private void SetScanningState(bool scanning)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var btn = this.FindControl<Button>("ScanButton");
            var spinner = this.FindControl<ProgressBar>("ScanProgress");
            if (btn != null) btn.IsEnabled = !scanning;
            if (spinner != null) spinner.IsVisible = scanning;
        });
    }

    private void CreatePlaylistFromScan_Click(object? sender, RoutedEventArgs e)
    {
        bool useCpu = ScannerSettings.UseCpuMetric;
        bool useRam = ScannerSettings.UseRamMetric;

        // Require at least one metric to be enabled
        if (!useCpu && !useRam)
        {
            SetStatus("Error: At least one metric (CPU or RAM) must be enabled in Settings.");
            return;
        }

        double cpuThreshold = ScannerSettings.CpuThreshold;
        double ramThreshold = ScannerSettings.RamThresholdMB;

        var qualifying = Results.Where(r =>
        {
            bool meetsCpu = !useCpu || r.CpuPercent > cpuThreshold;
            bool meetsRam = !useRam || r.MemoryMB > ramThreshold;
            return meetsCpu && meetsRam;
        }).ToList();

        if (qualifying.Count == 0)
        {
            string thresholdInfo = (useCpu, useRam) switch
            {
                (true, true) => $"{cpuThreshold}% CPU and {ramThreshold} MB RAM thresholds",
                (true, false) => $"{cpuThreshold}% CPU threshold",
                (false, true) => $"{ramThreshold} MB RAM threshold",
                _ => "configured thresholds"
            };
            SetStatus($"No apps exceeded the {thresholdInfo}. Try adjusting in Settings.");
            return;
        }

        var appNames = qualifying.Select(r => r.Name).ToList();
        var applications = string.Join(",", appNames);

        string description = (useCpu, useRam) switch
        {
            (true, true) => $"Auto-created from performance scan. CPU: >{cpuThreshold}%, RAM: >{ramThreshold} MB",
            (true, false) => $"Auto-created from performance scan. CPU threshold: {cpuThreshold}%",
            (false, true) => $"Auto-created from performance scan. RAM threshold: {ramThreshold} MB",
            _ => "Auto-created from performance scan."
        };

        var playlist = new PlaylistRecord(
            $"Scan Playlist — {DateTime.Now:HH:mm:ss}",
            description,
            applications);

        PlaylistState.Insert(playlist);

        string successInfo = (useCpu, useRam) switch
        {
            (true, true) => $"{qualifying.Count} app(s) above {cpuThreshold}% CPU and {ramThreshold} MB RAM",
            (true, false) => $"{qualifying.Count} app(s) above {cpuThreshold}% CPU",
            (false, true) => $"{qualifying.Count} app(s) above {ramThreshold} MB RAM",
            _ => $"{qualifying.Count} app(s)"
        };
        SetStatus($"Playlist created with {successInfo}.");
    }
}
