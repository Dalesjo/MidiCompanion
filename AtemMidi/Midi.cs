using RtMidi.Core;
using RtMidi.Core.Devices;
using RtMidi.Core.Devices.Infos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtemMidi;
public class Midi
{

    public static IMidiInputDevice GetMidiInput(string deviceName)
    {
        var deviceManager = MidiDeviceManager.Default;
        var inputDevice = deviceManager.InputDevices.FirstOrDefault(d => d.Name.Trim() == deviceName.Trim());

        if(inputDevice == null)
        {
            throw new Exception($"Device {deviceName} not found");
        }

        var device = inputDevice.CreateDevice();

        return device;
    }
}
