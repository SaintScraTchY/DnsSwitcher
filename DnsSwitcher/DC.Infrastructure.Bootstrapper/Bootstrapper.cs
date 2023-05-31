using DC.Application;
using DC.Application.Contracts.DnsObjContracts;
using DC.Domain.DnsObj;
using DC.Infrastructure.SQlite;
using DC.Infrastructure.SQlite.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DC.Infrastructure.Bootstrapper;

public class Bootstrapper
{
    public static IHostBuilder CreateConsoleHostBuilder()
    {
        return Host.CreateDefaultBuilder().ConfigureServices((context, services) =>
        {
            services.AddSingleton<IDnsObjApplication, DnsObjApplication>();
            services.AddSingleton<IDnsObjRepository, DnsObjRepository>();
            services.AddDbContext<DnsContext>(options => options.UseSqlite($"Data Source=DnsDB.db"));
        });
    }
}