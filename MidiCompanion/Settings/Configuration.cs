using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MidiCompanion.Settings;

public class Configuration
{

    [Required]
    public required string Companion { get; set; }

    [Required]
    public int Port { get; set; }

    [Required]
    public int LocalPort { get; set; }

    [Required]
    [ValidateEnumeratedItems]
    public required List<DeviceSetting> Devices { get; set; } = [];
}