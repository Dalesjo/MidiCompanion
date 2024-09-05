using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiCompanion.Settings;
public class OscButtonSetting
{
    [Required]
    public required int Page { get; set; }

    [Required]
    public required int Row { get; set; }

    [Required]
    public required int Column { get; set; }
}
