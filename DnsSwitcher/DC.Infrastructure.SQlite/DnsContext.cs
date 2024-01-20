using DC.Domain.DnsObj;
using DC.Infrastructure.SQlite.Mappings;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace DC.Infrastructure.SQlite;

public class DnsContext : DbContext
{
    public DnsContext(DbContextOptions<DnsContext> options) : base(options)
    {
    }

    public DbSet<DnsObj> Dnses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured) optionsBuilder.UseSqlite(@"Data Source=DnsDB.db");
        base.OnConfiguring(optionsBuilder);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var assembly = typeof(DnsObjMapping).Assembly;
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        base.OnModelCreating(modelBuilder);
    }
}