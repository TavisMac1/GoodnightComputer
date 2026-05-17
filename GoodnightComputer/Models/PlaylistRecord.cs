using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace GoodnightComputer.Models;

[Table("PlaylistEntities")]
public partial class PlaylistRecord : INotifyPropertyChanged
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    [Required]
    public string Name { get; init; } = string.Empty;

    public string? Descriptor { get; init; } = string.Empty;

    private bool _isActive = false;
    public bool IsActive 
    { 
        get => _isActive;
        set
        {
            if (_isActive != value)
            {
                _isActive = value;
                OnPropertyChanged();
            }
        }
    }

    public string CreatedOn { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

    [Required]
    public string Applications { get; set; } = string.Empty;

    private ObservableCollection<string> _applicationsList = new();
    [NotMapped]
    public ObservableCollection<string> ApplicationsList
    {
        get => _applicationsList;
        set
        {
            _applicationsList = value;
            OnPropertyChanged();
        }
    }

    [NotMapped]
    public ICommand DeleteFromDb { get; set; } = null!;

    [NotMapped]
    public ICommand SleepPlaylistApplications { get; set; } = null!;

    [NotMapped]
    public ICommand ResumePlaylistApplications { get; set; } = null!;

    [NotMapped]
    public ICommand EditPlaylist { get; set; } = null!;

    public PlaylistRecord() {}

    public PlaylistRecord
    (
        string name,
        string descriptor,
        string applicationRecords
    )
    {
        Applications = applicationRecords;
        Name = name;
        Descriptor = descriptor;
        ApplicationsList = new ObservableCollection<string>(applicationRecords.Split(new char[] { ',' }));
    }

    public PlaylistResult Validate(int applicationCount)
    {
        Console.WriteLine($"Name: {this.Name}");
        Console.WriteLine($"Descriptor: {this.Descriptor}");

        if (string.IsNullOrEmpty(Name)) return new("Name is empty.");
        if (applicationCount == 0) return new("There are no applications added.");

        return new(true);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
public class PlaylistResult
{
    public string ErrorMessage { get; set; } = "";
    public bool IsValid { get; set; }

    public PlaylistResult(string errorMessage)
    {
        ErrorMessage = errorMessage;
    }

    public PlaylistResult(bool isValid)
    {
        IsValid = isValid;
    }
}
