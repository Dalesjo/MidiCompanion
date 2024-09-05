using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MidiCompanion.Settings;
using RtMidi.Core;
using RtMidi.Core.Devices;
using RtMidi.Core.Enums;

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

        ConnectAllMidiDevices();


        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                foreach(var device in devices)
                {
                    if(!device.TestConnection())
                    {
                        log.LogWarning($"Device is disconnected. Reconnecting...");
                    }
                    else
                    {
                        log.LogWarning($"Device is connected...");
                    }
                }
                
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error in MidiWorker");
  
            }
            await Task.Delay(10000);
        }
    }

    private void DisconnectAllMidiDevices()
    {
        foreach (var device in devices)
        {
            device.Dispose();
        }

        devices.Clear();
    }

    private void ConnectAllMidiDevices()
    {
        var deviceManager = MidiDeviceManager.Default;
        var midiDevices = deviceManager.InputDevices;
        
        foreach (var midiDevice in midiDevices)
        {
            var deviceName = midiDevice.Name.Trim();
            var config = configuration.Value.Devices.FirstOrDefault(x => x.Device == deviceName);

            if(config == null)
            {
                log.LogInformation($"Available Device: '{deviceName}'");
                continue;
            }

            var deviceLogger = loggerFactory.CreateLogger<Device>();
            var device = new Device(config, sender, deviceLogger);
            device.Connect();
            devices.Add(device);

            log.LogInformation($"Available Device: '{deviceName}' (Connected)");
        }
    }

    private void PrintAllMidiDevices()
    {
        var deviceManager = MidiDeviceManager.Default;
        var inputDevices = deviceManager.InputDevices;

        foreach (var device in inputDevices)
        {
            log.LogInformation($"Available Device: '{device.Name}'");
        }
    }
}
