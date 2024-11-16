using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MidiCompanion.Settings;
using Rug.Osc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MidiCompanion;
public class OscSender : IDisposable
{
    public OscSender(
        ILogger<MidiWorker> log,
        IOptions<Configuration> configuration)
    {
        this.log = log;
        this.configuration = configuration;

        
        var address = IPAddress.Parse(configuration.Value.Companion);
        var port = configuration.Value.Port;
        var localPort = configuration.Value.LocalPort;

        log.LogInformation($"Connecting to Companion {address}:{port}");
        _sender = new Rug.Osc.OscSender(address, localPort,port);
        _sender.Connect();
    }

    private readonly IOptions<Configuration> configuration;
    private readonly ILogger<MidiWorker> log;
    private readonly Rug.Osc.OscSender _sender;

    public void Dispose()
    {
        _sender?.Dispose();
        GC.SuppressFinalize(this);
    }

    public void SetVariable(string variable, double value)
    {
        var address = $"/custom-variable/{variable}/value";
        var message = new OscMessage(address, value);

        
        _sender.Send(message);
        CheckState();
    }

    public void PressButton(int page, int row, int column)
    {
        Button(page, row, column, "PRESS");
    }
    
    public void DownButton(int page, int row, int column)
    {
        Button(page, row, column, "Down");
    }

    public void UpButton(int page, int row, int column)
    {
        Button(page, row, column, "Up");
    }

    public void RotateLeft(int page, int row, int column)
    {
        Button(page, row, column, "ROTATE-LEFT");
    }

    public void RotateRight(int page, int row, int column)
    {
        Button(page, row, column, "ROTATE-RIGHT");
    }

    public void Button(int page, int row, int column, string action)
    {
        var address = $"/location/{page}/{row}/{column}/{action}";
        var message = new OscMessage(address);

        CheckState();
        _sender.Send(message);
    }

    private void CheckState()
    {
        var state = _sender.State;

        log.LogInformation($"OscSender state: {state}");

        if(state != OscSocketState.Connected)
        {
            log.LogInformation("Reconnecting");
            _sender.Connect();
        }
    }

}
