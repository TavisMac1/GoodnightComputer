using Avalonia.Controls;
using Avalonia.Interactivity;
using GoodnightComputer.Controllers;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace GoodnightComputer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Initializes MainWindow and its components.
            InitializeComponent();
        }

        private void NewPlaylistView_Click(object? sender, RoutedEventArgs e)
        {
            this.FindControl<ContentControl>("MainContent").Content = new CreatePlaylist();
        }

        private void AddAppButton_Click(object? sender, RoutedEventArgs e)
        {
            // Opens the ListAllAppsModal when the Add App button is clicked.
            var modal = new ListAllAppsModal();
            this.Content = modal;
        }
    }
}