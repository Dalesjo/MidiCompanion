using Microsoft.Extensions.Logging;
using MidiCompanion.Settings;
using RtMidi.Core;
using RtMidi.Core.Devices;
using RtMidi.Core.Devices.Infos;
using RtMidi.Core.Messages;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;

namespace MidiCompanion;
internal class Device(
    DeviceSetting deviceSetting,
    OscSender sender,
    ILogger<Device> log) : IDisposable
{
    private MidiInPort? device;
    private DeviceWatcher? deviceWatcher;
    private string? deviceId;

    public async Task Connect()
    {
        await Reconnect();
        MonitorDevice();
    }

    private void MonitorDevice()
    {
        deviceWatcher = DeviceInformation.CreateWatcher(MidiInPort.GetDeviceSelector());

        deviceWatcher.Added += async (DeviceWatcher sender, DeviceInformation args) =>
        {
            if (deviceId != args.Id)
            {
                return;
            }

            if(sender.Status == DeviceWatcherStatus.Started)
            {
                return;
            }

            log.LogInformation($"Device added: {args.Name} DeviceId '{args.Id}'");
            await Reconnect();
        };

        deviceWatcher.Removed += (DeviceWatcher sender, DeviceInformationUpdate args) =>
        {
            if (deviceId == args.Id)
            {
                log.LogInformation($"Device removed: DeviceId '{args.Id}'");
                Disconnect();
            }
        };

        deviceWatcher.Start();
    }

    public async Task Reconnect()
    {
        try
        {
            if (device != null)
            {
                Disconnect();
            }

            if (deviceSetting.DeviceId == null)
            {
                deviceId = await FindDeviceId(deviceSetting.Name);
            }
            else
            {
                deviceId = deviceSetting.DeviceId;
            }

            if (deviceId == null)
            {
                log.LogError($"Device {deviceSetting.Name} not found");
                return;
            }

            device = await MidiInPort.FromIdAsync(deviceId);

            if (device == null)
            {
                log.LogInformation($"Connection failed for '{deviceSetting.Name}' with DeviceId '{deviceId}'");
                return;
            }

            device.MessageReceived += MessageRecieved;

            log.LogInformation($"Connected to '{deviceSetting.Name}' with DeviceId '{deviceId}'");
        } 
        finally
        {

        }
    }

    public void Disconnect()
    {
        if (device == null)
        {
            return;
        }

        device.MessageReceived -= MessageRecieved;
        device.Dispose();
        device = null;
    }

    private void MessageRecieved(MidiInPort sender, MidiMessageReceivedEventArgs args)
    {
        var midiMessage = args.Message;
        if (midiMessage.Type != MidiMessageType.ControlChange)
        {
            return;
        }

        var controlChangeMessage = (MidiControlChangeMessage)midiMessage;
        HandleMessage(controlChangeMessage);
    }

    private static async Task<string?> FindDeviceId(string name)
    {
        var midiDevices = await DeviceInformation.FindAllAsync(MidiInPort.GetDeviceSelector());

        foreach (var midiDevice in midiDevices)
        {
            if (midiDevice.Name.Trim() == name.Trim())
            {
                return midiDevice.Id;
            }
        }

        return null;
    }

    public void Dispose()
    {
        if(device == null)
        {
            return;
        }

        device.Dispose();
        device = null;
    }

    void HandleMessage(MidiControlChangeMessage msg)
    {
        var channel = (byte)(msg.Channel+1);
        var control = msg.Controller;
        var value = msg.ControlValue;

        var button = deviceSetting.Buttons.FirstOrDefault(x => x.Midi.Channel == channel && x.Midi.ControlChange == control);
        if (button != null)
        {
            SendButton(button, channel, control, value);
            return;
        }

        var fader = deviceSetting.Faders.FirstOrDefault(x => x.Midi.Channel == channel && x.Midi.ControlChange == control);
        if (fader != null)
        {
            SendFader(fader, channel, control, value);
            return;
        }

        var rotary = deviceSetting.Rotary.FirstOrDefault(x => x.Midi.Channel == channel && x.Midi.ControlChange == control);
        if (rotary != null)
        {
            SendRotary(rotary, channel, control, value);
            return;
        }

        var push = deviceSetting.Push.FirstOrDefault(x => x.Midi.Channel == channel && x.Midi.ControlChange == control);
        if(push != null)
        {
            SendPush(push, channel, control, value);
            return;
        }

        log.LogWarning($"No Fader/button/rotary found for channel {channel} and control {control} with value {msg.ControlValue}");
        return;
    }

    private void SendPush(PushSetting push, byte channel, byte control, byte value)
    {
        

        if(value == 127)
        {
            log.LogInformation($"Channel {channel}, CC {control} with value {value}, push step 1 {push.Osc.Page}/{push.Osc.Row}/{push.Osc.Column}");
            sender.Step(push.Osc.Page, push.Osc.Row, push.Osc.Column, 1);
        }
        else
        {
            log.LogInformation($"Channel {channel}, CC {control} with value {value}, push step 2 {push.Osc.Page}/{push.Osc.Row}/{push.Osc.Column}");
            sender.Step(push.Osc.Page, push.Osc.Row, push.Osc.Column, 0);
        }

        sender.Button(push.Osc.Page, push.Osc.Row, push.Osc.Column, "PRESS");
    }

    private void SendRotary(RotarySetting rotary, byte channel, byte control, byte value)
    {
        if (value == rotary.Midi.Max)
        {
            log.LogInformation($"Channel {channel}, CC {control} with value {value}, rotate right {rotary.Osc.Page}/{rotary.Osc.Row}/{rotary.Osc.Column}");
            sender.RotateRight(rotary.Osc.Page, rotary.Osc.Row, rotary.Osc.Column);
        }

        if (value == rotary.Midi.Min)
        {
            log.LogInformation($"Channel {channel}, CC {control} with value {value}, rotate left {rotary.Osc.Page}/{rotary.Osc.Row}/{rotary.Osc.Column}");
            sender.RotateLeft(rotary.Osc.Page, rotary.Osc.Row, rotary.Osc.Column);
        }
    }

    private void SendButton(ButtonSetting button, byte channel, byte control, byte value)
    {
        if(value == button.Midi.Max)
        {
            log.LogInformation($"Channel {channel}, CC {control} with value {value}, press button {button.Osc.Page}/{button.Osc.Row}/{button.Osc.Column}");
            sender.DownButton(button.Osc.Page, button.Osc.Row, button.Osc.Column);
        }

        if (value == button.Midi.Min)
        {
            log.LogInformation($"Channel {channel}, CC {control} with value {value}, release button {button.Osc.Page}/{button.Osc.Row}/{button.Osc.Column}");
            sender.UpButton(button.Osc.Page, button.Osc.Row, button.Osc.Column);
        }
    }

    private void SendFader(FaderSetting fader, byte channel, byte control, byte value)
    {
        var oscValue = fader.MidiToOSCValue(value);
        log.LogInformation($"Channel {channel}, CC {control} with value {value}, Set Variable $(internal:custom_{fader.Osc.Variable}) to value {oscValue}, press and release button {fader.Osc.Page}/{fader.Osc.Row}/{fader.Osc.Column}");

        sender.SetVariable(fader.Osc.Variable, oscValue);
        sender.PressButton(fader.Osc.Page, fader.Osc.Row, fader.Osc.Column);
    }
}
