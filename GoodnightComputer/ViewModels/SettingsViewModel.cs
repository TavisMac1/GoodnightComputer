using Avalonia.Controls;
using Avalonia.Interactivity;
using GoodnightComputer.Services;
using GoodnightComputer.State;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GoodnightComputer.ViewModels;

public partial class SettingsViewModel : UserControl
{
    public ObservableCollection<string> UserExclusions { get; } = new(UserExclusionState.Exclusions);

    public SettingsViewModel()
    {
        InitializeComponent();
        DataContext = this;

        // Load CPU threshold if user-defined
        if (ScannerSettings.IsUserDefined)
        {
            var cpuInput = this.FindControl<TextBox>("CpuThresholdInput");
            if (cpuInput != null)
                cpuInput.Text = ScannerSettings.CpuThreshold.ToString();
        }

        // Load RAM threshold
        var ramInput = this.FindControl<TextBox>("RamThresholdInput");
        if (ramInput != null)
            ramInput.Text = ScannerSettings.RamThresholdMB.ToString();

        // Load metric toggles
        var useCpuCheckBox = this.FindControl<CheckBox>("UseCpuCheckBox");
        if (useCpuCheckBox != null)
            useCpuCheckBox.IsChecked = ScannerSettings.UseCpuMetric;

        var useRamCheckBox = this.FindControl<CheckBox>("UseRamCheckBox");
        if (useRamCheckBox != null)
            useRamCheckBox.IsChecked = ScannerSettings.UseRamMetric;

        // Set the initial language selection
        var languageComboBox = this.FindControl<ComboBox>("LanguageComboBox");
        if (languageComboBox != null)
        {
            var currentLanguage = LocalizationService.Instance.CurrentLanguage;
            var selectedItem = LocalizationService.Instance.AvailableLanguages
                .FirstOrDefault(kvp => kvp.Key == currentLanguage);
            languageComboBox.SelectedItem = selectedItem;
        }
    }

    private void SaveCpuThreshold_Click(object? sender, RoutedEventArgs e)
    {
        var input = this.FindControl<TextBox>("CpuThresholdInput");
        if (input == null || string.IsNullOrWhiteSpace(input.Text)) return;

        if (double.TryParse(input.Text.Trim(), out double value) && value >= 0 && value <= 100)
            ScannerSettings.CpuThreshold = value;
    }

    private void SaveRamThreshold_Click(object? sender, RoutedEventArgs e)
    {
        var input = this.FindControl<TextBox>("RamThresholdInput");
        if (input == null || string.IsNullOrWhiteSpace(input.Text)) return;

        if (double.TryParse(input.Text.Trim(), out double value) && value >= 0)
            ScannerSettings.RamThresholdMB = value;
    }

    private void MetricToggle_Changed(object? sender, RoutedEventArgs e)
    {
        var useCpuCheckBox = this.FindControl<CheckBox>("UseCpuCheckBox");
        var useRamCheckBox = this.FindControl<CheckBox>("UseRamCheckBox");

        if (useCpuCheckBox != null)
            ScannerSettings.UseCpuMetric = useCpuCheckBox.IsChecked ?? true;

        if (useRamCheckBox != null)
            ScannerSettings.UseRamMetric = useRamCheckBox.IsChecked ?? false;
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

    private void LanguageComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox && comboBox.SelectedItem is KeyValuePair<string, string> selected)
        {
            LocalizationService.Instance.CurrentLanguage = selected.Key;
        }
    }
}
