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
        private ListBox SelectedAppsListBoxControl;
        private ListBox AppsListBoxControl;
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
            SelectedAppsListBoxControl = this.FindControl<ListBox>("SelectedAppsListBox");
            AppsListBoxControl = this.FindControl<ListBox>("AppsListBox");
            SearchTextBoxControl = this.FindControl<TextBox>("SearchTextBox");
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
        }

        public void RemoveApp_Click(object? sender, RoutedEventArgs e)
        {
            var listBox = this.FindControl<ListBox>("SelectedAppsListBox");
            var selectedApp = listBox.SelectedItems?.OfType<ApplicationRecord>().First();

            if (selectedApp != null)
            { 
                SelectedApps.Remove(selectedApp);
            }
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

            ErrorMessageControl.Text = string.Empty;
        }

        public void SavePlaylist_Click(object? sender, RoutedEventArgs e)
        {
            var Playlist = new PlaylistRecord(
                PlaylistNameTextBoxControl.Text,
                PlaylistDescriptionTextBoxControl.Text,
                GeneralUtils.PrepareApplicationRecords(SelectedApps.ToList())
            );

            var playListResult = Playlist.Validate(SelectedAppsListBoxControl.SelectedItems.Count);

            if (!playListResult.IsValid) ErrorMessageControl.Text = playListResult.ErrorMessage;

            Playlist.Save();
               
            ErrorMessageControl.Text = "";
        }
    }
}