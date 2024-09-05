using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AtemMidi;
public class Configuration
{
   
    [Required]
    public required string Companion { get; set; }

    [Required]
    public int Port { get; set; }

    [Required]
    [ValidateEnumeratedItems]
    public required List<Converter> Converters { get; set; }
}

public class Converter
{
    [Required]
    [ValidateObjectMembers]
    public required MidiSetting Midi { get; set; }

    [Required]
    [ValidateObjectMembers]
    public required OscSetting Osc { get; set; }
}

public class  MidiSetting
{
    [Required]
    public required string Device { get; set; }

    [Required]
    public required int Channel { get; set; }

    [Required]
    public required int ControlChange { get; set; }

    [Required]
    public required int Min { get; set; }

    [Required]
    public required int Max { get; set; }
}

public class OscSetting
{
    [Required]
    public string Variable { get; set; }

    [Required]
    public required int Min { get; set; }

    [Required]
    public required int Max { get; set; }
}
