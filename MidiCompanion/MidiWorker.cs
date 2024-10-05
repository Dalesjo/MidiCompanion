using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MidiCompanion.Settings;
using RtMidi.Core;
using RtMidi.Core.Devices;
using Windows.Devices.Enumeration;
using Windows.Devices.Midi;

namespace MidiCompanion;
public class MidiWorker(
    ILogger<MidiWorker> log,
    IOptions<Configuration> configuration,
    OscSender sender,
    ILoggerFactory loggerFactory
    ) : BackgroundService
{

    private readonly List<Device> devices = [];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await PrintAllMidiDevices();
        await ConnectAllMidiDevices();
        await stoppingToken.WaitForCancellationAsync();
        DisconnectAllMidiDevices();
    }

    private void DisconnectAllMidiDevices()
    {
        foreach (var device in devices)
        {
            device.Dispose();
        }

        devices.Clear();
    }

    private async Task ConnectAllMidiDevices()
    {


        foreach (var deviceConfig in configuration.Value.Devices)
        {
            var deviceLogger = loggerFactory.CreateLogger<Device>();
            var device = new Device(deviceConfig, sender, deviceLogger);
            await device.Connect();
            devices.Add(device);

        }

    }

    
    private async Task PrintAllMidiDevices()
    {
        var midiDevices = await DeviceInformation.FindAllAsync(MidiInPort.GetDeviceSelector());
        foreach (var midiDevice in midiDevices)
        {
            log.LogInformation($"Available Device: '{midiDevice.Name}', Id: '{midiDevice.Id}'");
        }
    }
}
