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
        
        _toastService.Show("Testing a toast! toast toast toast toast toast", 3000, new AvaloniaToaster.Themes.AvaloniaToasterSuccessTheme());
        _toastService.Show("Where am i??");
    }

    public void OnDeletePlaylist() =>
        _toastService.Show("Playlist deleted!", 3000, new AvaloniaToaster.Themes.AvaloniaToasterSuccessTheme());
    public void OnPlayPlaylist() =>
        _toastService.Show("Starting Playlist");
}