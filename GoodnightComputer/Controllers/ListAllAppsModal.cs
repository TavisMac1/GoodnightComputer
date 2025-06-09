using Avalonia.Controls;
using Avalonia.Interactivity;
using GoodnightComputer.Utils;

namespace GoodnightComputer.Controllers
{
    public partial class ListAllAppsModal : UserControl
    {
        public ListAllAppsModal()
        {
            // Initializes ListAllAppsModal and its components.
            InitializeComponent();

            AppsListBox.ItemsSource = GeneralUtils.ListApplicationNames();
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            // Your logic here (e.g., hide the modal)
        }

        private void OkButton_Click(object? sender, RoutedEventArgs e)
        {
            // Your logic here (e.g., hide the modal)
        }
    }
}
