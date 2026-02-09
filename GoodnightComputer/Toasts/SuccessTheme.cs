using Avalonia.Layout;
using Avalonia.Media;
using AvaloniaToaster.Interfaces;

namespace GoodnightComputer.Toasts;

internal class SuccessTheme : IAvaloniaToasterThemes
{
    public IBrush BackgroundColor => Avalonia.Media.Brushes.Black;

    public IBrush ForegroundColor => Avalonia.Media.Brushes.Green;

    HorizontalAlignment? IAvaloniaToasterThemes.HorizontalAlignment => HorizontalAlignment.Right;

    VerticalAlignment? IAvaloniaToasterThemes.VerticalAlignment => VerticalAlignment.Bottom;

    double? IAvaloniaToasterThemes.BorderRadius => null;
}

