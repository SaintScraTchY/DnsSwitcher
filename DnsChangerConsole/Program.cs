// See https://aka.ms/new-console-template for more information

using DC.Application;
using DC.Application.Contracts.DnsObjContracts;
using DC.Domain.DnsObj;
using DC.Infrastructure.SQlite;
using DC.Infrastructure.SQlite.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DnsChangerConsole;

internal class Program
{
    private const string DbName = "DnsDB.db";
    public static IServiceProvider ServiceProvider { get; private set; }

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
        consoleapp.GoHome();
        Console.ReadLine();
    }

    private static async Task<IHost> BuildHost()
    {
        var host = Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
        {
            services.AddSingleton<IDnsObjApplication, DnsObjApplication>();
            services.AddSingleton<IDnsObjRepository, DnsObjRepository>();

            services.AddDbContext<DnsContext>(options => options.UseSqlite(@$"Data Source={DbName}"));
        }).ConfigureLogging(Logger => Logger.ClearProviders()).Build();
        return host;
    }
}