using Avalonia.Controls;
using Avalonia.Interactivity;
using GoodnightComputer.ViewModels;

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
    }
}