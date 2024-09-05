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
        var address = IPAddress.Parse(configuration.Value.Companion);
        var port = configuration.Value.Port;

        _sender = new Rug.Osc.OscSender(address, port);
        _sender.Connect();
    }

    private readonly Rug.Osc.OscSender _sender;

    public void Dispose()
    {
        _sender?.Dispose();
        GC.SuppressFinalize(this);
    }

    public void SetVariable(string variable,double value)
    {
        var address = $"/custom-variable/{variable}/value";
        var message = new OscMessage(address, value);

        _sender.Send(message);
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

        _sender.Send(message);
    }

}
