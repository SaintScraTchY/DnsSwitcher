using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QD.Core.Application;
using QD.Core.Application.NetworkInterfaceHelper;
using QD.Core.Cotracts.DnsObjContracts;
using QD.Core.Domain.DnsObj;
using QD.Core.Infrastructure.SQLItePCL;
using Spectre.Console;

namespace QuickDNSConsole;

internal static class Program
{
    private const string DbName = "DnsDB.db";

    public static void Main(string[] args)
    {
        IServiceProvider serviceProvider = null;
        AnsiConsole.Status().Spinner(Spinner.Known.Arc).Start("Getting Things Ready...", ctc =>
        {
            var host = BuildHost();
            serviceProvider = host.Services;
            Thread.Sleep(1000);
        });
        var consoleApp = serviceProvider.GetRequiredService<ConsoleApp>();
    }

    private static IHost BuildHost()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton<DnsContext>(
                    db => ActivatorUtilities.CreateInstance<DnsContext>(db, DbName));
                services.AddSingleton<IDnsObjRepository, DnsObjRepository>();
                services.AddSingleton<IDnsObjApplication, DnsObjApplication>();
                services.AddSingleton<NetworkInterfaceClass>();
                services.AddSingleton<ConsoleApp>();
            })
            .Build();
    }
}
