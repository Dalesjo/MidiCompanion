using LibAtem.Commands.Audio;
using LibAtem.Commands.Audio.Fairlight;
using LibAtem.Common;
using LibAtem.Net;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtemMidi;

public class AudioAtem(
    AtemClient atemClient)
{

    public void SetAudioLevel()
    {
        var command = new FairlightMixerSourceSetCommand
        {
            Index = AudioSource.Mic2,
            SourceId = -65280,
            FaderGain = -14,
        };

        atemClient.SendCommand(command);

        var command2 = new FairlightMixerSourceSetCommand
        {
            Index = AudioSource.Input1,
            SourceId = -65280,
            FaderGain = -14,
        };

        atemClient.SendCommand(command2);
    }

    private static double MapValue(double inputValue, double inputMin, double inputMax, double outputMin, double outputMax)
    {
        // Ensure the input range is valid
        if (inputMin == inputMax)
        {
            throw new ArgumentException("inputMin and inputMax cannot be the same value.");
        }

        // Calculate the ratio of the input value within the input range
        double ratio = (inputValue - inputMin) / (inputMax - inputMin);

        // Map the ratio to the output range
        double outputValue = outputMin + (ratio * (outputMax - outputMin));

        return outputValue;
    }
}
