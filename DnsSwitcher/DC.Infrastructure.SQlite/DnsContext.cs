using DC.Domain.DnsObj;
using DC.Infrastructure.SQlite.Mappings;
using HelperClass.Application;
using Microsoft.EntityFrameworkCore;

namespace DC.Infrastructure.SQlite;

public class DnsContext : DbContext
{
    public DbSet<DnsObj> Dnses { get; set; }

    public DnsContext(DbContextOptions<DnsContext> options) : base(options)
    {
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        optionsBuilder.UseSqlite(@"Data Source=J:\Dev\Projects\CSharp\DotNetCore\DnsChanger\DnsChangerConsole\DnsDB.db");
        base.OnConfiguring(optionsBuilder );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assembly = typeof(DnsObjMapping).Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        base.OnModelCreating(modelBuilder);
    }
}