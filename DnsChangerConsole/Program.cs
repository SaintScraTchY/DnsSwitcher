using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DC.Core.Application;
using DC.Core.Application.NetworkInterfaceHelper;
using DC.Core.Cotracts.DnsObjContracts;
using DC.Core.Domain.DnsObj;
using DC.Core.Infrastructure.SQLItePCL;

namespace DnsChangerConsole;

internal static class Program
{
    private const string DbName = "DnsDB.db";

    public static async Task Main(string[] args)
    {
        //string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbName);

        using var host = BuildHost();
        var services = host.Services;

        Console.WriteLine("Getting Things Ready...");

        var consoleApp = services.GetRequiredService<ConsoleApp>();
        //consoleApp.RunAsync(CancellationToken.None); // synchronous loop
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
