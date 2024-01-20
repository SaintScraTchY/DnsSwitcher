using DC.Domain.DnsObj;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DC.Infrastructure.SQlite.Mappings;

public class DnsObjMapping : IEntityTypeConfiguration<DnsObj>
{
    public void Configure(EntityTypeBuilder<DnsObj> builder)
    {
        builder.ToTable("Dnses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name);
        builder.Property(x => x.DnsAddresses);
        builder.Property(x => x.CreationDate);
        builder.Property(x => x.ModifiedDate);
    }
}