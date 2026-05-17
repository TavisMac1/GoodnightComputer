using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Threading;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoodnightComputer.Views;

public partial class SplashScreen : Window
{
    public event Action? AnimationComplete;

    private readonly CancellationTokenSource _skipCts = new();

    public SplashScreen()
    {
        InitializeComponent();
        Opened += OnOpened;
        KeyDown      += (_, _) => _skipCts.Cancel();
        PointerPressed += (_, _) => _skipCts.Cancel();
    }

    private async void OnOpened(object? sender, EventArgs e)
    {
        var logoBorder   = this.FindControl<Border>("LogoBorder")!;
        var titleText    = this.FindControl<TextBlock>("TitleText")!;
        var subtitleText = this.FindControl<TextBlock>("SubtitleText")!;
        var skip         = _skipCts.Token;

        // ── Initial state ────────────────────────────────────────────────
        Opacity = 0;
        logoBorder.RenderTransformOrigin = new Avalonia.RelativePoint(0.5, 0.5, Avalonia.RelativeUnit.Relative);
        logoBorder.RenderTransform = new ScaleTransform(0.55, 0.55);
        SetSlideTransform(titleText,    28);
        SetSlideTransform(subtitleText, 28);

        try
        {
            // ── Phase 1: fade-in window + scale+fade logo (0.8s) ─────────
            var fadeIn  = MakeOpacityAnimation(0, 1, 0.8, new CubicEaseOut());
            var scaleTcs = new TaskCompletionSource();
            RunScaleTimer(logoBorder, from: 0.55, to: 1.0, durationSec: 0.8, easePow: 3, tcs: scaleTcs);

            await Task.WhenAll(fadeIn.RunAsync(this, skip), scaleTcs.Task.WaitAsync(skip));

            // ── Phase 2: title slides up + fades in (0.45s) ──────────────
            await Task.WhenAll(
                MakeOpacityAnimation(0, 1, 0.45, new CubicEaseOut()).RunAsync(titleText, skip),
                RunSlideTimer(titleText, fromY: 28, toY: 0, durationSec: 0.45, easePow: 3).WaitAsync(skip)
            );

            // ── Phase 3: subtitle fades + slides in (0.35s) ──────────────
            await Task.Delay(60, skip);
            await Task.WhenAll(
                MakeOpacityAnimation(0, 1, 0.35, new CubicEaseOut()).RunAsync(subtitleText, skip),
                RunSlideTimer(subtitleText, fromY: 28, toY: 0, durationSec: 0.35, easePow: 3).WaitAsync(skip)
            );

            // ── Hold ──────────────────────────────────────────────────────
            await Task.Delay(TimeSpan.FromSeconds(0.9), skip);
        }
        catch (OperationCanceledException)
        {
            // User skipped — snap everything to final visible state instantly
            Opacity = 1;
            ((ScaleTransform)logoBorder.RenderTransform).ScaleX = 1;
            ((ScaleTransform)logoBorder.RenderTransform).ScaleY = 1;
            titleText.Opacity    = 1;
            subtitleText.Opacity = 1;
            ((TranslateTransform)titleText.RenderTransform!).Y    = 0;
            ((TranslateTransform)subtitleText.RenderTransform!).Y = 0;
        }

        // ── Phase 4: fade out (quick when skipped) ────────────────────────
        var fadeOut = MakeOpacityAnimation(1, 0, 0.35, new CubicEaseIn());
        await fadeOut.RunAsync(this, CancellationToken.None);

        AnimationComplete?.Invoke();
        Close();
    }

    // ── Helpers ──────────────────────────────────────────────────────────

    private static Animation MakeOpacityAnimation(double from, double to, double seconds, Easing easing)
        => new Animation
        {
            Duration = TimeSpan.FromSeconds(seconds),
            FillMode = FillMode.Forward,
            Easing   = easing,
            Children =
            {
                new KeyFrame { Cue = new Cue(0d), Setters = { new Setter(OpacityProperty, from) } },
                new KeyFrame { Cue = new Cue(1d), Setters = { new Setter(OpacityProperty, to)   } },
            }
        };

    private static void SetSlideTransform(Control control, double offsetY)
    {
        control.RenderTransformOrigin = new Avalonia.RelativePoint(0.5, 0.5, Avalonia.RelativeUnit.Relative);
        control.RenderTransform = new TranslateTransform(0, offsetY);
    }

    private static Task RunSlideTimer(Control control, double fromY, double toY, double durationSec, double easePow)
    {
        var tcs       = new TaskCompletionSource();
        var startTime = DateTime.UtcNow;
        var duration  = TimeSpan.FromSeconds(durationSec);

        DispatcherTimer.Run(() =>
        {
            var t     = Math.Clamp((DateTime.UtcNow - startTime).TotalSeconds / duration.TotalSeconds, 0, 1);
            var eased = 1.0 - Math.Pow(1.0 - t, easePow);
            var y     = fromY + (toY - fromY) * eased;

            ((TranslateTransform)control.RenderTransform!).Y = y;

            if (t >= 1.0) { tcs.TrySetResult(); return false; }
            return true;
        }, TimeSpan.FromMilliseconds(16));

        return tcs.Task;
    }

    private static void RunScaleTimer(Control control, double from, double to, double durationSec, double easePow, TaskCompletionSource tcs)
    {
        var startTime = DateTime.UtcNow;
        var duration  = TimeSpan.FromSeconds(durationSec);

        DispatcherTimer.Run(() =>
        {
            var t     = Math.Clamp((DateTime.UtcNow - startTime).TotalSeconds / duration.TotalSeconds, 0, 1);
            var eased = 1.0 - Math.Pow(1.0 - t, easePow);
            var scale = from + (to - from) * eased;

            ((ScaleTransform)control.RenderTransform!).ScaleX = scale;
            ((ScaleTransform)control.RenderTransform!).ScaleY = scale;

            if (t >= 1.0) { tcs.TrySetResult(); return false; }
            return true;
        }, TimeSpan.FromMilliseconds(16));
    }
}