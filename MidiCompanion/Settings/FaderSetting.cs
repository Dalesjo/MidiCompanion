using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace MidiCompanion.Settings;
public class FaderSetting
{
    [Required]
    [ValidateObjectMembers]
    public required MidiSetting Midi { get; set; }

    [Required]
    [ValidateObjectMembers]
    public required OscSetting Osc { get; set; }

    public double MidiToOSCValue(double inputValue)
    {
        // Ensure the input range is valid
        if (Midi.Min == Midi.Max)
        {
            throw new ArgumentException("Midi.Min and inputMax cannot be the same value.");
        }

        var ratio = (inputValue - Midi.Min) / (Midi.Max - Midi.Min);
        var output = Osc.Min + (ratio * (Osc.Max - Osc.Min));

        if (Osc.Decimals == false)
        {
            output = Math.Round(output);
        }

        if (output < Osc.Min)
        {
            return Osc.Min;
        }

        if (output > Osc.Max)
        {
            return Osc.Max;
        }

        return output;
    }
}
