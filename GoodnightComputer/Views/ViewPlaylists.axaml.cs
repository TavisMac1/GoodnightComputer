using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaToaster;
using FuzzySharp;
using GoodnightComputer.Models;
using GoodnightComputer.State;
using GoodnightComputer.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace GoodnightComputer.Views;
public partial class ViewPlaylists : UserControl
{
    private readonly ToastNotificationService _toastService;
    private ViewPlaylistsViewModel _vm = null!;

    private ListBox _editAvailableAppsListBox = null!;
    private ListBox _editSelectedAppsListBox = null!;
    private TextBox _editSearchTextBox = null!;

    public ViewPlaylists()
    {    
        InitializeComponent();
        _vm = Program.Services.GetRequiredService<ViewPlaylistsViewModel>();
        DataContext = _vm;
        this._toastService = Program.Services.GetRequiredService<ToastNotificationService>();

        this.Loaded += ViewPlaylists_Loaded;
    }

    private void ViewPlaylists_Loaded(object? sender, RoutedEventArgs e)
    {
        _editAvailableAppsListBox = this.FindControl<ListBox>("EditAvailableAppsListBox")!;
        _editSelectedAppsListBox = this.FindControl<ListBox>("EditSelectedAppsListBox")!;
        _editSearchTextBox = this.FindControl<TextBox>("EditSearchTextBox")!;

        var editAddBtn = this.FindControl<Button>("EditAddAppButton")!;
        var editRemoveBtn = this.FindControl<Button>("EditRemoveAppButton")!;
        var editSaveBtn = this.FindControl<Button>("EditSaveButton")!;
        var editSearchBtn = this.FindControl<Button>("EditSearchButton")!;
        var editRefreshBtn = this.FindControl<Button>("EditRefreshButton")!;

        editAddBtn.Click += EditAddApp_Click;
        editRemoveBtn.Click += EditRemoveApp_Click;
        editSaveBtn.Click += EditSave_Click;
        editSearchBtn.Click += EditSearch_Click;
        editRefreshBtn.Click += EditRefresh_Click;
    }

    private void EditSearch_Click(object? sender, RoutedEventArgs e)
    {
        var input = _editSearchTextBox?.Text ?? string.Empty;
        _vm.ModalAvailableApps.Clear();

        var source = string.IsNullOrWhiteSpace(input)
            ? SystemState.STATIC_APPLICATION_NAMES
            : SystemState.STATIC_APPLICATION_NAMES
                .Where(app => Fuzz.PartialTokenSetRatio(app, input) >= Constants.Constants.SCORE_THRESHOLD)
                .ToList();

        foreach (var app in source)
            _vm.ModalAvailableApps.Add(new ApplicationRecord(app));
    }

    private async void EditRefresh_Click(object? sender, RoutedEventArgs e)
    {
        var freshNames = await System.Threading.Tasks.Task.Run(Utils.GeneralUtils.ListApplicationNames);
        SystemState.STATIC_APPLICATION_NAMES = freshNames;

        _vm.ModalAvailableApps.Clear();
        foreach (var app in freshNames)
            _vm.ModalAvailableApps.Add(new ApplicationRecord(app));

        if (_editSearchTextBox != null)
            _editSearchTextBox.Text = string.Empty;
    }

    private void EditAddApp_Click(object? sender, RoutedEventArgs e)
    {
        var selected = _editAvailableAppsListBox?.SelectedItems?.OfType<ApplicationRecord>().ToList()
                       ?? new List<ApplicationRecord>();

        if (selected.Count == 0)
        {
            _toastService.Show("Please select at least one application to add.", 3000, new AvaloniaToaster.Themes.AvaloniaToasterErrorTheme());
            return;
        }

        foreach (var app in selected)
        {
            if (!_vm.ModalSelectedApps.Any(a => a.Name == app.Name))
            {
                _vm.ModalSelectedApps.Add(app);
            }
            else
            { 
                _toastService.Show($"'{app.Name}' is already in the playlist.", 3000, new AvaloniaToaster.Themes.AvaloniaToasterErrorTheme());
            }
        }
    }

    private void EditRemoveApp_Click(object? sender, RoutedEventArgs e)
    {
        var selected = _editSelectedAppsListBox?.SelectedItem as ApplicationRecord;
        if (selected == null)
        {
            _toastService.Show("Please select an application to remove.", 3000, new AvaloniaToaster.Themes.AvaloniaToasterErrorTheme());
            return;
        }
        _vm.ModalSelectedApps.Remove(selected);
    }

    private void EditSave_Click(object? sender, RoutedEventArgs e)
    {
        if (_vm.EditingPlaylist == null) return;

        if (_vm.ModalSelectedApps.Count == 0)
        {
            _toastService.Show("There are no applications added.", 3000, new AvaloniaToaster.Themes.AvaloniaToasterErrorTheme());
            return;
        }

        var newList = _vm.ModalSelectedApps.Select(a => a.Name).ToList();
        PlaylistState.UpdateApplications(_vm.EditingPlaylist.Id, newList);

        _vm.IsEditModalOpen = false;
        _toastService.Show("Playlist updated!");
    }

    public void OnDeletePlaylist() =>
        _toastService.Show("Playlist deleted!", 3000, new AvaloniaToaster.Themes.AvaloniaToasterSuccessTheme());
    public void OnPlayPlaylist() =>
        _toastService.Show("Starting Playlist");
}
