// See https://aka.ms/new-console-template for more information

using System;
using System.IO;
using DC.Application;
using DC.Application.Contracts.DnsObjContracts;
using DC.Domain.DnsObj;
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
        private const string  DbName = "DnsDB.db";
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Getting Things Ready...");

            if (!File.Exists(DbName))
            {
                File.Create(DbName);
                Console.WriteLine("Please Restart the App for full initialization");
            }
            var host = await BuildHost();
            ServiceProvider = host.Services;

            if (File.Exists(DbName))
            {
                ServiceProvider.GetRequiredService<DnsContext>().Database.Migrate();
            }
            
            ConsoleApp consoleapp = new ConsoleApp(ServiceProvider.GetRequiredService<IDnsObjApplication>());
            consoleapp.GoHome();
            Console.ReadLine();
        }
        
        private static async Task<IHost> BuildHost()
        {
             IHost host = Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
             {
                 services.AddTransient<IDnsObjApplication, DnsObjApplication>();
                 services.AddTransient<IDnsObjRepository, DnsObjRepository>();
                
                 services.AddDbContext<DnsContext>(options => options.UseSqlite(@$"Data Source={DbName}"));
             }).ConfigureLogging(Logger => Logger.ClearProviders()).Build();
             return host;
        }

    }
}

