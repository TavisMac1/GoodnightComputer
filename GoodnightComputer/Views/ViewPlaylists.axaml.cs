using Avalonia.Controls;
using GoodnightComputer.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace GoodnightComputer.Views;
public partial class ViewPlaylists : UserControl
{
    public ViewPlaylists()
    {       
        InitializeComponent();
        DataContext = Program.Services.GetRequiredService<ViewModels.ViewPlaylistsViewModel>();
    }
}