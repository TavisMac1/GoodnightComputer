using Avalonia.Controls;
namespace GoodnightComputer.Utils;
public static class UIUtils
{
    public static void FreezeUi(Avalonia.Controls.Window window)
    {
        var overlay = window.FindControl<Border>("LoadingOverlay");
        if (overlay != null)
            overlay.IsVisible = true;
    }

    public static void UnFreezeUi(Avalonia.Controls.Window window)
    {
        var overlay = window.FindControl<Border>("LoadingOverlay");
        if (overlay != null)
            overlay.IsVisible = false;
    }
}
