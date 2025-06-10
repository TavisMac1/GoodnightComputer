using Avalonia.Controls;
using Avalonia.Interactivity;
using GoodnightComputer.Models;
using GoodnightComputer.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GoodnightComputer.ViewModels
{
    public partial class CreatePlaylist : UserControl
    {
        public ObservableCollection<ApplicationRecord> AddedApplicationList { get; set; } = new();
        public ObservableCollection<ApplicationRecord> ApplicationList { get; set; } = new();

        public CreatePlaylist()
        {
            InitializeComponent();
            ApplicationList.Add(new ApplicationRecord("Random Data"));
            /*
            foreach (var app in GeneralUtils.ListApplicationNames())
            {
                ApplicationList.Add(new ApplicationRecord(app));
            }
            */

            applicationsList.ItemsSource = ApplicationList;
        }

        public void AddAppToPlaylist_Click(object? sender, RoutedEventArgs e)
        { 
            
        }

        public void OpenAppPicker_Click(object? sender, RoutedEventArgs e)
        {
            // Logic to open the application picker modal
            var modal = new ListAllAppsModal();
            this.FindControl<ContentControl>("MainContent").Content = modal;
        }

        public void SavePlaylist_Click(object? sender, RoutedEventArgs e)
        {
            var errorMessageControl = this.FindControl<TextBlock>("ErrorMessageTextBlock");
            var name = this.FindControl<TextBox>("PlaylistNameTextBox").Text;
            var description = this.FindControl<TextBox>("PlaylistDescriptionTextBox").Text;
            List<ApplicationRecord> apps = this.FindControl<ListBox>("AppsListBox").SelectedItems as List<ApplicationRecord>;

            var isValid = PlaylistUtils.ValidatePlaylist(name, description, apps);

            if (!isValid)
            {
                // Show an error message to the user
                errorMessageControl.Text = "Please fill in all fields and select at least one application.";
                return;
            }

            var PlaylistRecord = new PlaylistRecord
                (
                    name,
                    description,
                    apps
                );

            // Save to SQLite 
            errorMessageControl.Text = "";
        }
    }
}
