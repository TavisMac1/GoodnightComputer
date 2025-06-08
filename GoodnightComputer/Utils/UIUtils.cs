using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodnightComputer.Utils
{
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
}
