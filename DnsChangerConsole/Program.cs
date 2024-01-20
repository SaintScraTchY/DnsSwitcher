using DC.Application;
using DC.Application.Contracts.DnsObjContracts;
using DC.Application.NetworkInterfaceHelper;
using DC.Domain.DnsObj;
using DC.Infrastructure.SQlite;
using DC.Infrastructure.SQlite.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DnsChangerConsole;

internal static class Program
{
    private const string DbName = "DnsDB.db";
    private static IServiceProvider ServiceProvider { get; set; }

    public static async Task Main(string[] args)
    {
        var host = await BuildHost();
        Console.WriteLine("Getting Things Ready...");

        if (!File.Exists(DbName))
        {
            File.Create(DbName);
            Console.WriteLine("Please Restart the App for full initialization");
        }

        ServiceProvider = host.Services;

        if (File.Exists(DbName)) ServiceProvider.GetRequiredService<DnsContext>().Database.Migrate();

        var consoleapp = new ConsoleApp(ServiceProvider.GetRequiredService<IDnsObjApplication>());
        await consoleapp.GoHome();
        Console.ReadLine();
    }

    private static async Task<IHost> BuildHost()
    {
        var host = Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
        {
            services.AddTransient<NetworkInterfaceClass>();
            services.AddTransient<IDnsObjRepository, DnsObjRepository>();
            services.AddTransient<IDnsObjApplication, DnsObjApplication>();

            services.AddDbContext<DnsContext>(options => options.UseSqlite(@$"Data Source={DbName}"));
        }).ConfigureLogging(Logger => Logger.ClearProviders()).Build();
        return host;
    }
}