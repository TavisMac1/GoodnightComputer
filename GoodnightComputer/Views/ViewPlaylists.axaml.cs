using Avalonia.Controls;
using AvaloniaToaster;
using GoodnightComputer.Toasts;
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
        
        _toastService.Show("Testing a toast!", 3000, new SuccessTheme());
    }
}