using RtMidi.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiCompanion.Settings;
public class MidiSetting
{
    [Required]
    public required byte Channel { get; set; }

    [Required]
    public required byte ControlChange { get; set; }

    [Required]
    public required byte Min { get; set; } = 0;

    [Required]
    public required byte Max { get; set; } = 127;
}
