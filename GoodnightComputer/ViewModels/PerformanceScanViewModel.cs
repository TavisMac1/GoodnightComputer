using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using GoodnightComputer.Models;
using GoodnightComputer.Utils;
using System;
using System.Collections.ObjectModel;
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
}
