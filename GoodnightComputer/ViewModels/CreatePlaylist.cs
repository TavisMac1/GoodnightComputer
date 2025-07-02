using Avalonia.Controls;
using Avalonia.Interactivity;
using GoodnightComputer.Models;
using GoodnightComputer.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GoodnightComputer.ViewModels
{
    public partial class CreatePlaylist : UserControl
    {
        public ObservableCollection<ApplicationRecord> SelectedApps { get; set; } = new();
        public ObservableCollection<ApplicationRecord> ApplicationList { get; set; } = new();

        public CreatePlaylist()
        {
            InitializeComponent();
            DataContext = this;
            Initialize();

            /*
            foreach (var app in GeneralUtils.ListApplicationNames())
            {
                ApplicationList.Add(new ApplicationRecord(app));
            }
            */
            //applicationsList.ItemsSource = ApplicationList;
        }

        private void Initialize()
        {
            foreach (var app in GeneralUtils.ListApplicationNames())
            {
                ApplicationList.Add(new ApplicationRecord(app));
            }
        }

        public void AddAppToPlaylist_Click(object? sender, RoutedEventArgs e)
        {
            var errorMessageControl = this.FindControl<TextBlock>("ErrorMessageTextBlock");
            var listBox = this.FindControl<ListBox>("AppsListBox");

            // For multiple selection (recommended)
            var selectedApps = listBox.SelectedItems?.OfType<ApplicationRecord>().ToList() ?? new List<ApplicationRecord>();

            if (selectedApps.Count == 0)
            {
                errorMessageControl.Text = "Please select at least one application to add.";
                return;
            }

            // Add selected apps to SelectedApps collection (avoid duplicates)
            foreach (var app in selectedApps)
            {
                if (!SelectedApps.Contains(app))
                    SelectedApps.Add(app);
            }

            errorMessageControl.Text = ""; // Clear error on success
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