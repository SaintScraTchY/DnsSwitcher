// See https://aka.ms/new-console-template for more information

using DC.Application;
using DC.Application.Contracts.DnsObjContracts;
using DC.Domain.DnsObj;
using DC.Infrastructure.Bootstrapper;
using DC.Infrastructure.SQlite;
using DC.Infrastructure.SQlite.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DnsChangerConsole
{
    class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public static void Main(string[] args)
        {
            Console.WriteLine("Getting Things Ready...");
            IHost host = Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
            {
                services.AddTransient<IDnsObjApplication, DnsObjApplication>();
                services.AddTransient<IDnsObjRepository, DnsObjRepository>();
                services.AddDbContext<DnsContext>(options => options.UseSqlite(@"Data Source=J:\Dev\Projects\CSharp\DotNetCore\DnsChanger\DnsChangerConsole\DnsDB.db"));
            }).ConfigureLogging(Logger => Logger.ClearProviders()).Build();
            
            ServiceProvider = host.Services;

            ConsoleApp consoleapp = new ConsoleApp(ServiceProvider.GetRequiredService<IDnsObjApplication>());
            consoleapp.GoHome();
            Console.ReadKey();
        }

    }
}

