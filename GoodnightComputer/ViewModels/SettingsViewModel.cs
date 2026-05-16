using Avalonia.Controls;
using Avalonia.Interactivity;
using GoodnightComputer.State;
using System.Collections.ObjectModel;

namespace GoodnightComputer.ViewModels;

public partial class SettingsViewModel : UserControl
{
    public ObservableCollection<string> UserExclusions { get; } = new(UserExclusionState.Exclusions);

    public SettingsViewModel()
    {
        InitializeComponent();
        DataContext = this;

        if (ScannerSettings.IsUserDefined)
        {
            var input = this.FindControl<TextBox>("ThresholdInput");
            if (input != null)
                input.Text = ScannerSettings.CpuThreshold.ToString();
        }
    }

    private void SaveThreshold_Click(object? sender, RoutedEventArgs e)
    {
        var input = this.FindControl<TextBox>("ThresholdInput");
        if (input == null || string.IsNullOrWhiteSpace(input.Text)) return;

        if (double.TryParse(input.Text.Trim(), out double value) && value >= 0 && value <= 100)
            ScannerSettings.CpuThreshold = value;
    }

    private void AddExclusion_Click(object? sender, RoutedEventArgs e)
    {
        var input = this.FindControl<TextBox>("ExclusionInput");
        if (input == null || string.IsNullOrWhiteSpace(input.Text)) return;

        var value = input.Text.Trim();
        UserExclusionState.Add(value);
        if (!UserExclusions.Contains(value.ToLower()))
            UserExclusions.Add(value.ToLower());

        input.Text = string.Empty;
    }

    private void RemoveExclusion_Click(object? sender, RoutedEventArgs e)
    {
        var list = this.FindControl<ListBox>("ExclusionsList");
        if (list?.SelectedItem is string selected)
        {
            UserExclusionState.Remove(selected);
            UserExclusions.Remove(selected);
        }
    }
}
