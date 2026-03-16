using Avalonia.Controls;
using AvaloniaToaster;
using Microsoft.Extensions.DependencyInjection;

namespace GoodnightComputer.Views;
public partial class ViewPlaylists : UserControl
{
    private readonly ToastNotificationService _toastService;
    
    public ViewPlaylists()
    {    
        InitializeComponent();
        DataContext = Program.Services.GetRequiredService<ViewModels.ViewPlaylistsViewModel>();
        this._toastService = Program.Services.GetRequiredService<ToastNotificationService>();
    }

    public void OnDeletePlaylist() =>
        _toastService.Show("Playlist deleted!", 3000, new AvaloniaToaster.Themes.AvaloniaToasterSuccessTheme());
    public void OnPlayPlaylist() =>
        _toastService.Show("Starting Playlist");
}