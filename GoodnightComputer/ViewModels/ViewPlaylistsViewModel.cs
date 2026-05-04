using GoodnightComputer.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

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

    public ViewPlaylistsViewModel()
    {
        this.PlaylistRecords = PlaylistState.GlobalPlaylistRecords;
        
        // Subscribe to property changes on all playlist records
        foreach (var record in PlaylistRecords)
        {
            if (record is INotifyPropertyChanged notifyRecord)
            {
                notifyRecord.PropertyChanged += Record_PropertyChanged;
            }
        }
    }

    private void Record_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PlaylistRecord.IsActive))
        {
            UpdateActivePlaylist();
        }
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
