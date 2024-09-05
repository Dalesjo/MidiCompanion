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
    public required Channel Channel { get; set; }

    [Required]
    public required int ControlChange { get; set; }

    [Required]
    public required int Min { get; set; } = 0;

    [Required]
    public required int Max { get; set; } = 127;
}
