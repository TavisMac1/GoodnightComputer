using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Windows.Input;

namespace GoodnightComputer.Models;

[Table("PlaylistEntities")]
public partial class PlaylistRecord
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; }

    [Required]
    public string Name { get; init; } = string.Empty;
    
    [Required]
    public string Descriptor { get; init; } = string.Empty;

    public bool IsActive { get; set; } = false;

    public string CreatedOn { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

    [Required]
    public string Applications { get; init; } = string.Empty;

    public List<string> ApplicationsList { get; init; }

    [NotMapped]
    public ICommand DeleteFromDb { get; set; } = null!;

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
        ApplicationsList = applicationRecords.Split(new char[] { ',' }).ToList();
    }

    public PlaylistResult Validate(int applicationCount)
    {
        Console.WriteLine($"Name: {this.Name}");
        Console.WriteLine($"Descriptor: {this.Descriptor}");

        if (string.IsNullOrEmpty(Name)) return new("Name is empty.");
        if (string.IsNullOrEmpty(Descriptor)) return new("Description is empty");
        if (applicationCount == 0) return new("There are no applications added.");

        return new(true);
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
