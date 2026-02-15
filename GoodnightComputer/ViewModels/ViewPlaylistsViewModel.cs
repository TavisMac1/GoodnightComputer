using GoodnightComputer.Models;
using System.Collections.ObjectModel;

namespace GoodnightComputer.ViewModels;

public class ViewPlaylistsViewModel
{
    public ObservableCollection<PlaylistRecord> PlaylistRecords { get; set; }

    public ViewPlaylistsViewModel(PlaylistState playlistStateSingleton) =>
        this.PlaylistRecords = playlistStateSingleton.GlobalPlaylistRecords;
}
