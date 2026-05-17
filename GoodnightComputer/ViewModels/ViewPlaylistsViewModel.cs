using GoodnightComputer.Models;
using GoodnightComputer.State;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace GoodnightComputer.ViewModels;

public class ViewPlaylistsViewModel : INotifyPropertyChanged
{
    public ObservableCollection<PlaylistRecord> PlaylistRecords { get; set; }

    private PlaylistRecord? _activePlaylist;
    public PlaylistRecord? ActivePlaylist
    {
        get => _activePlaylist;
        set
        {
            if (_activePlaylist != value)
            {
                _activePlaylist = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsAnyPlaylistActive));
            }
        }
    }

    public bool IsAnyPlaylistActive => ActivePlaylist != null;

    // --- Edit Modal State ---

    private bool _isEditModalOpen = false;
    public bool IsEditModalOpen
    {
        get => _isEditModalOpen;
        set { _isEditModalOpen = value; OnPropertyChanged(); }
    }

    private PlaylistRecord? _editingPlaylist;
    public PlaylistRecord? EditingPlaylist
    {
        get => _editingPlaylist;
        set { _editingPlaylist = value; OnPropertyChanged(); }
    }

    public ObservableCollection<ApplicationRecord> ModalAvailableApps { get; set; } = new();
    public ObservableCollection<ApplicationRecord> ModalSelectedApps { get; set; } = new();

    public ICommand CloseEditModalCommand { get; }

    public ViewPlaylistsViewModel()
    {
        this.PlaylistRecords = PlaylistState.GlobalPlaylistRecords;

        foreach (var record in PlaylistRecords)
        {
            if (record is INotifyPropertyChanged notifyRecord)
                notifyRecord.PropertyChanged += Record_PropertyChanged;
        }

        CloseEditModalCommand = new RelayCommand(_ => IsEditModalOpen = false, _ => true);

        PlaylistState.OpenEditModalAction = OpenEditModal;
    }

    public void OpenEditModal(PlaylistRecord playlist)
    {
        EditingPlaylist = playlist;

        ModalAvailableApps.Clear();
        foreach (var app in SystemState.STATIC_APPLICATION_NAMES)
            ModalAvailableApps.Add(new ApplicationRecord(app));

        ModalSelectedApps.Clear();
        foreach (var appName in playlist.ApplicationsList)
            ModalSelectedApps.Add(new ApplicationRecord(appName));

        IsEditModalOpen = true;
    }

    private void Record_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PlaylistRecord.IsActive))
            UpdateActivePlaylist();
    }

    private void UpdateActivePlaylist()
    {
        ActivePlaylist = PlaylistRecords.FirstOrDefault(p => p.IsActive);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}