using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiCompanion.Settings;
public class PushSetting
{
    [Required]
    [ValidateObjectMembers]
    public required MidiSetting Midi { get; set; }

    [Required]
    [ValidateObjectMembers]
    public required OscRotarySetting Osc { get; set; }
}
