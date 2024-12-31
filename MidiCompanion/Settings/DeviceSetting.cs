using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiCompanion.Settings;
public class DeviceSetting
{
    [Required]
    public required string Name { get; set; }

    public string? DeviceId { get; set;}

    [Required]
    [ValidateEnumeratedItems]
    public required List<FaderSetting> Faders { get; set; } = [];

    [Required]
    [ValidateEnumeratedItems]
    public required List<ButtonSetting> Buttons { get; set; } = [];

    [Required]
    [ValidateEnumeratedItems]
    public required List<RotarySetting> Rotary { get; set; } = [];

    [Required]
    [ValidateEnumeratedItems]
    public required List<PushSetting> Push { get; set; } = [];


}
