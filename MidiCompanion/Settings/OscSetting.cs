using System.ComponentModel.DataAnnotations;
namespace MidiCompanion.Settings;

public class OscSetting
{
    [Required]
    public required string Variable { get; set; }

    [Required]
    public required int Min { get; set; }

    [Required]
    public required int Max { get; set; }
    
    
    [Required]
    public required bool Decimals { get; set; }

    [Required]
    public required int Page { get; set; }

    [Required]
    public required int Row { get; set; }

    [Required]
    public required int Column { get; set; }
}
