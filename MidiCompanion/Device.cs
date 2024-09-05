using Microsoft.Extensions.Logging;
using MidiCompanion.Settings;
using RtMidi.Core;
using RtMidi.Core.Devices;
using RtMidi.Core.Messages;

namespace MidiCompanion;
internal class Device(
    DeviceSetting deviceSetting,
    OscSender sender,
    ILogger<Device> log) : IDisposable
{
    private IMidiInputDevice? device;

    public void Connect()
    {
        device = GetMidiInput(deviceSetting.Device);
        device.ControlChange += ControlChangeHandler;
        device.Open();
    }


    public void Dispose()
    {
        if(device == null)
        {
            return;
        }

        device.Close();

    }

    public bool TestConnection()
    {
        return device?.IsOpen ?? false;
    }

    void ControlChangeHandler(IMidiInputDevice device, in ControlChangeMessage msg)
    {
        var channel = msg.Channel;
        var control = msg.Control;

        var button = deviceSetting.Buttons.FirstOrDefault(x => x.Midi.Channel == channel && x.Midi.ControlChange == control);
        if (button != null)
        {
            SendButton(button, msg);
            return;
        }

        var fader = deviceSetting.Faders.FirstOrDefault(x => x.Midi.Channel == channel && x.Midi.ControlChange == control);
        if (fader != null)
        {
            SendFader(fader, msg);
            return;
        }

        var rotary = deviceSetting.Rotary.FirstOrDefault(x => x.Midi.Channel == channel && x.Midi.ControlChange == control);
        if (rotary != null)
        {
            SendRotary(rotary, msg);
            return;
        }

        log.LogWarning($"No Fader/button/rotary found for channel {channel} and control {control} with value {msg.Value}");
        return;
    }

    private void SendRotary(RotarySetting rotary, ControlChangeMessage msg)
    {
        if (msg.Value == rotary.Midi.Max)
        {
            log.LogInformation($"Channel {msg.Channel}, CC {msg.Control} with value {msg.Value}, rotate right {rotary.Osc.Page}/{rotary.Osc.Row}/{rotary.Osc.Column}");
            sender.RotateRight(rotary.Osc.Page, rotary.Osc.Row, rotary.Osc.Column);
        }

        if (msg.Value == rotary.Midi.Min)
        {
            log.LogInformation($"Channel {msg.Channel}, CC {msg.Control} with value {msg.Value}, rotate left {rotary.Osc.Page}/{rotary.Osc.Row}/{rotary.Osc.Column}");
            sender.RotateLeft(rotary.Osc.Page, rotary.Osc.Row, rotary.Osc.Column);
        }
    }

    private void SendButton(ButtonSetting button, ControlChangeMessage msg)
    {
        if(msg.Value == button.Midi.Max)
        {
            log.LogInformation($"Channel {msg.Channel}, CC {msg.Control} with value {msg.Value}, press button {button.Osc.Page}/{button.Osc.Row}/{button.Osc.Column}");
            sender.DownButton(button.Osc.Page, button.Osc.Row, button.Osc.Column);
        }

        if (msg.Value == button.Midi.Min)
        {
            log.LogInformation($"Channel {msg.Channel}, CC {msg.Control} with value {msg.Value}, release button {button.Osc.Page}/{button.Osc.Row}/{button.Osc.Column}");
            sender.UpButton(button.Osc.Page, button.Osc.Row, button.Osc.Column);
        }
    }

    private void SendFader(FaderSetting fader, in ControlChangeMessage msg)
    {
        var oscValue = fader.MidiToOSCValue(msg.Value);
        log.LogInformation($"Channel {msg.Channel}, CC {msg.Control} with value {msg.Value}, Set Variable {fader.Osc.Variable} to value {oscValue}, press and release button {fader.Osc.Page}/{fader.Osc.Row}/{fader.Osc.Column}");

        sender.SetVariable(fader.Osc.Variable, oscValue);
        sender.PressButton(fader.Osc.Page, fader.Osc.Row, fader.Osc.Column);
    }

    private static IMidiInputDevice GetMidiInput(string deviceName)
    {
        var deviceManager = MidiDeviceManager.Default;
        var inputDevice = deviceManager.InputDevices.FirstOrDefault(d => d.Name.Trim() == deviceName.Trim());

        if (inputDevice == null)
        {
            throw new Exception($"Device {deviceName} not found");
        }

        var device = inputDevice.CreateDevice();

        return device;
    }
}
