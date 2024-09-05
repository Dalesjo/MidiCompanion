

using AtemMidi;
using LibAtem.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using AtemMidi;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
    options.SingleLine = true;
});

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "DalesjoMidi";
});

builder.Services.AddSingleton(di => new AtemClient("192.168.50.123"));
builder.Services.AddSingleton<AudioAtem>();
builder.Services.AddSingleton<Sender>();
builder.Services.AddHostedService<MidiWorker>();
builder.Services.AddHostedService<AtemDebug>();

var host = builder.Build();


await host.RunAsync();