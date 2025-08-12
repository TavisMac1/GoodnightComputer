using Avalonia.Controls;
using Avalonia.Interactivity;
using GoodnightComputer.Models;
using GoodnightComputer.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GoodnightComputer.ViewModels
{
    public partial class CreatePlaylist : UserControl
    {
        public ObservableCollection<ApplicationRecord> SelectedApps { get; set; } = new();
        public ObservableCollection<ApplicationRecord> ApplicationList { get; set; } = new();
        public ObservableCollection<ApplicationRecord> CachedApplicationList { get; set; } = new();

        private TextBlock ErrorMessageControl;
        private TextBox PlaylistNameTextBoxControl;
        private TextBox PlaylistDescriptionTextBoxControl;
        private ListBox AppsListBoxControl;
        private ListBox SelectedAppsListBoxControl;
        private TextBox SearchTextBoxControl;

        public CreatePlaylist()
        {
            InitializeComponent();
            DataContext = this;
            Initialize();
        }

        private void Initialize()
        {
            foreach (var app in GeneralUtils.ListApplicationNames())
            {
                ApplicationList.Add(new ApplicationRecord(app));
            }

            ErrorMessageControl = this.FindControl<TextBlock>("ErrorMessageTextBlock");
            PlaylistNameTextBoxControl = this.FindControl<TextBox>("PlaylistNameTextBox");
            PlaylistDescriptionTextBoxControl = this.FindControl<TextBox>("PlaylistDescriptionTextBox");
            AppsListBoxControl = this.FindControl<ListBox>("AppsListBox");
            SelectedAppsListBoxControl = this.FindControl<ListBox>("SelectedAppsListBox");
            SearchTextBoxControl = this.FindControl<TextBox>("SearchTextBox");

            if (ErrorMessageControl == null) throw new Exception("ErrorMessageTextBlock not found.");
            if (PlaylistNameTextBoxControl == null) throw new Exception("PlaylistNameTextBox not found.");
            if (PlaylistDescriptionTextBoxControl == null) throw new Exception("PlaylistDescriptionTextBox not found.");
            if (AppsListBoxControl == null) throw new Exception("AppsListBox not found.");
            if (SelectedAppsListBoxControl == null) throw new Exception("SelectedAppsListBox not found.");
            if (SearchTextBoxControl == null) throw new Exception("SearchTextBox not found.");
            ErrorMessageControl.Text = string.Empty;
        }

        private void Search(string input)
        {
            CachedApplicationList = ApplicationList;

            if (ApplicationList.Count > 0)
            {
                ApplicationList = new ObservableCollection<ApplicationRecord>(
                    ApplicationList.Where(app => app.Name.Contains(
                            input, 
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                );
            }
        }

        public void SearchApps_Click(object? sender, RoutedEventArgs e)
        {
            var searchText = SearchTextBoxControl.Text;

            if (string.IsNullOrEmpty(searchText))
            {
                ErrorMessageControl.Text = "Please enter input in the search box.";
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
                ErrorMessageControl.Text = "Please select an application to remove.";
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
                ErrorMessageControl.Text = "Please select at least one application to add.";
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
            var Playlist = new PlaylistRecord(
                PlaylistNameTextBoxControl.Text,
                PlaylistDescriptionTextBoxControl.Text,
                GeneralUtils.PrepareApplicationRecords(SelectedApps.ToList())
            );

            var playListResult = Playlist.Validate(
                    SelectedApps.Count
                );

            if (!playListResult.IsValid)
            {
                ErrorMessageControl.Text = playListResult.ErrorMessage;
                return;
            }

            Playlist.Save();

            ClearErrorState();
        }

        private void ClearErrorState() => this.ErrorMessageControl.Text = string.Empty;
    }
}