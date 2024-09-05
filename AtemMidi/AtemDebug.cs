using LibAtem.Commands;
using LibAtem.Commands.Audio.Fairlight;
using LibAtem.Commands.DataTransfer;
using LibAtem.Net;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AtemMidi;
public class AtemDebug : BackgroundService
{
    private readonly AtemClient _atemClient;
    private readonly JsonSerializerOptions options = new() { WriteIndented = true };
    public AtemDebug(AtemClient atemClient)
    {
        _atemClient = atemClient;
    }


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _atemClient.OnReceive += AtemClient_OnReceived;
        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken stoppingToken)
    {
        _atemClient.OnReceive -= AtemClient_OnReceived;
        return base.StopAsync(stoppingToken);
    }

    private void AtemClient_OnReceived(object sender, IReadOnlyList<ICommand> commands)
    {
        
        foreach (var command in commands)
        {
            if(command is TimeCodeCommand)
            {
                continue;
            }

            if (command is DataTransferCompleteCommand)
            {
                continue;
            }

            DeserializeIfPossible(command);

            if (command is FairlightMixerSourceGetCommand fairlightMixerSourceGetCommand)
            {
                Console.WriteLine("FaderGain:"+ fairlightMixerSourceGetCommand.FaderGain);
                Console.WriteLine("MaxFramesDelay:" + fairlightMixerSourceGetCommand.MaxFramesDelay);
                Console.WriteLine("FramesDelay:" + fairlightMixerSourceGetCommand.FramesDelay);
            }


            
        }
    }

    private void DeserializeIfPossible(ICommand command)
    {
        try
        {
            var commandType = command.GetType();
            Console.WriteLine(commandType.Name);
            var json = JsonSerializer.Serialize(command, commandType, options);
            Console.WriteLine(json);
        }
        catch(Exception)
        {

           Console.WriteLine("Could not serialize");
        }
    }
}
