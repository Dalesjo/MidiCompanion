using Rug.Osc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AtemMidi;
public class Sender : IDisposable
{
    public Sender()
    {
        var address = IPAddress.Parse("192.168.50.53");
        var port = 12321;

        _sender = new OscSender(address, port);
        _sender.Connect();
    }

    private readonly OscSender _sender;

    public void Dispose()
    {
        _sender?.Dispose();
    }

    public void Send(int value)
    {
        var variableName = "oscvalue";
        var oscAddress = $"/custom-variable/{variableName}/value";

        var message = new OscMessage(oscAddress, value);

        _sender.Send(message);
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
