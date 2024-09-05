using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MidiCompanion.Settings;
using MidiCompanion;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
    options.SingleLine = true;
});

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "MidiCompanion";
});

builder.Services.AddOptions<Configuration>()
    .Bind(builder.Configuration.GetRequiredSection("Configuration"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<OscSender>();
builder.Services.AddHostedService<MidiWorker>();

var host = builder.Build();
await host.RunAsync();