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
