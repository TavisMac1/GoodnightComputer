using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaToaster;
using GoodnightComputer.Models;
using GoodnightComputer.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace GoodnightComputer.ViewModels;
public partial class CreatePlaylist : UserControl
{
    private readonly PlaylistState _playlistStateSingleton;
    private readonly ToastNotificationService _toastService;

    public ObservableCollection<ApplicationRecord> SelectedApps { get; set; } = new();
    public ObservableCollection<ApplicationRecord> ApplicationList { get; set; } = new();

    private TextBlock MessageControl;
    private TextBox PlaylistNameTextBoxControl;
    private TextBox PlaylistDescriptionTextBoxControl;
    private ListBox AppsListBoxControl;
    private ListBox SelectedAppsListBoxControl;
    private TextBox SearchTextBoxControl;

    public CreatePlaylist
    (
        ToastNotificationService toastService, 
        PlaylistState playlistState
    )
    { 
        _toastService = toastService;
        this._playlistStateSingleton = playlistState;
        InitializeComponent();
        DataContext = this;
        Initialize();
    }

    private async Task Initialize()
    {
        foreach (var app in State.SystemState.STATIC_APPLICATION_NAMES) 
            ApplicationList.Add(new ApplicationRecord(app));
        
        MessageControl = this.FindControl<TextBlock>("ErrorMessageTextBlock");
        PlaylistNameTextBoxControl = this.FindControl<TextBox>("PlaylistNameTextBox");
        PlaylistDescriptionTextBoxControl = this.FindControl<TextBox>("PlaylistDescriptionTextBox");
        AppsListBoxControl = this.FindControl<ListBox>("AppsListBox");
        SelectedAppsListBoxControl = this.FindControl<ListBox>("SelectedAppsListBox");
        SearchTextBoxControl = this.FindControl<TextBox>("SearchTextBox");

        if (MessageControl == null) throw new Exception("ErrorMessageTextBlock not found.");
        if (PlaylistNameTextBoxControl == null) throw new Exception("PlaylistNameTextBox not found.");
        if (PlaylistDescriptionTextBoxControl == null) throw new Exception("PlaylistDescriptionTextBox not found.");
        if (AppsListBoxControl == null) throw new Exception("AppsListBox not found.");
        if (SelectedAppsListBoxControl == null) throw new Exception("SelectedAppsListBox not found.");
        if (SearchTextBoxControl == null) throw new Exception("SearchTextBox not found.");
        MessageControl.Text = string.Empty;
    }

    private void Search(string input)
    {
        if (ApplicationList.Count > 0)
        {
            var filtered = ApplicationList.Where
            (
                app => app.Name.Contains(input, StringComparison.OrdinalIgnoreCase)
            ).ToList();

            ApplicationList.Clear();

            foreach (var app in filtered)
                ApplicationList.Add(app);
        }
    }

    public void SearchApps_Click(object? sender, RoutedEventArgs e)
    {
        var searchText = SearchTextBoxControl.Text;

        if (string.IsNullOrEmpty(searchText))
        {
            ApplicationList.Clear();
            State.SystemState.STATIC_APPLICATION_NAMES.ForEach
            (
                (app) => { ApplicationList.Add(new ApplicationRecord(app)); }
            );
            return;
        }

        Search(searchText);
        ClearErrorState();
    }

    public void RemoveApp_Click(object? sender, RoutedEventArgs e)
    {
        var selectedApp = null as ApplicationRecord;

        if (SelectedAppsListBoxControl == null) throw new Exception("No application selected to remove.");

        if (SelectedAppsListBoxControl.SelectedItems != null && SelectedAppsListBoxControl.SelectedItems.Count == 0) 
        {
            MessageControl.Text = "Please select an application to remove.";
            return;
        }

        if (SelectedAppsListBoxControl.SelectedItem != null) selectedApp = SelectedAppsListBoxControl.SelectedItem as ApplicationRecord;
        else if (SelectedAppsListBox.SelectedItems.Count > 0) selectedApp = SelectedAppsListBoxControl.SelectedItems?.OfType<ApplicationRecord>().First(); // this assumes there's multi select - should just disable it

        SelectedApps.Remove(selectedApp);
        ClearErrorState();
    }

    public void AddAppToPlaylist_Click(object? sender, RoutedEventArgs e)
    {
        var selectedApps = AppsListBoxControl.SelectedItems?.OfType<ApplicationRecord>().ToList() ?? new List<ApplicationRecord>();

        if (selectedApps.Count == 0)
        {
            MessageControl.Text = "Please select at least one application to add.";
            return;
        }

        foreach (var app in selectedApps)
        {
            if (!SelectedApps.Contains(app))
                SelectedApps.Add(app);
        }

        ClearErrorState();
    }

    public void SavePlaylist_Click(object? sender, RoutedEventArgs e)
    {
        var Playlist = new PlaylistRecord
        (
            PlaylistNameTextBoxControl.Text,
            PlaylistDescriptionTextBoxControl.Text,
            GeneralUtils.PrepareApplicationRecords(SelectedApps.ToList())
        );

        var playListResult = Playlist.Validate
        (
            SelectedApps.Count
        );

        if (!playListResult.IsValid)
        {
            MessageControl.Text = playListResult.ErrorMessage;
            return;
        }

        _playlistStateSingleton.Insert(Playlist);
        _toastService.Show("Playlist saved!");
        ClearErrorState();
    }

    private void ClearErrorState() => this.MessageControl.Text = string.Empty;
}