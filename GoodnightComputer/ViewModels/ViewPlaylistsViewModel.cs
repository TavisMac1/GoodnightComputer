using GoodnightComputer.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GoodnightComputer.ViewModels;

public class ViewPlaylistsViewModel
{
    public ObservableCollection<PlaylistRecord> PlaylistRecords { get; set; }

    public ViewPlaylistsViewModel(PlaylistState playlistStateSingleton)
    {
        this.PlaylistRecords = playlistStateSingleton.GlobalPlaylistRecords;
    }
}
