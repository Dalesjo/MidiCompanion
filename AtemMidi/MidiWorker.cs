using Microsoft.Extensions.Hosting;
using RtMidi.Core.Devices;
using RtMidi.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RtMidi.Core.Messages;
using Microsoft.Extensions.Logging;
using LibAtem.Net;
using MidiCompanion;

namespace AtemMidi;
public class MidiWorker(
    ILogger<MidiWorker> log,
    Sender sender,
    AudioAtem audioAtem
    ) : BackgroundService
{



    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        
        while(!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(10000, stoppingToken);
            try
            {
                log.LogInformation("MidiWorker running at: {time}", DateTimeOffset.Now);
                audioAtem.SetAudioLevel();
            } catch(Exception ex)
            {
                log.LogError(ex, "Error in MidiWorker");
            }
            
        }

        await stoppingToken.WaitForCancellationAsync();

    }

}
