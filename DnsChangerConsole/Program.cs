using DC.Application;
using DC.Application.Contracts.DnsObjContracts;
using DC.Application.NetworkInterfaceHelper;
using DC.Domain.DnsObj;
using DC.Infrastructure.SQlitePCL;
using DC.Infrastructure.SQlitePCL.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace DnsChangerConsole;

internal static class Program
{
    private const string DbName = "DnsDB.db";

    public static async Task Main(string[] args)
    {
        string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DbName);
        IServiceProvider ServiceProvider;
        var host = await BuildHost();
        ServiceProvider = host.Services;
        Console.WriteLine("Getting Things Ready...");

        // if (!File.Exists(DbName))
        // {
        //     File.Create(DbName);
        //     Console.WriteLine("Please Restart the App for full initialization");
        // }

        //if (File.Exists(DbName)) ServiceProvider.GetRequiredService<DnsContext>().Database.Migrate();

        var consoleapp = new ConsoleApp(ServiceProvider.GetRequiredService<IDnsObjApplication>());
        consoleapp.GoHome();
        Console.ReadLine();
    }

    private static async Task<IHost> BuildHost()
    {
        var host = Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
        {
            services.AddSingleton<NetworkInterfaceClass>();
            services.AddSingleton<IDnsObjRepository, DnsObjRepository>();
            services.AddSingleton<IDnsObjApplication, DnsObjApplication>();

            services.AddSingleton<DnsContext>(db => ActivatorUtilities.CreateInstance<DnsContext>(db,DbName));

            //services.AddDbContext<DnsContext>(options => options.UseSqlite(@$"Data Source={DbName}"));
            
        }).Build();
        return host;
    }
}