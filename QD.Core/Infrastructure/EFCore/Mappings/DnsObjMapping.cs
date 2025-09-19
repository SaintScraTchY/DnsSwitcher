using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QD.Core.Domain.DnsObj;

namespace QD.Core.Infrastructure.EFCore.Mappings;

public class DnsObjMapping : IEntityTypeConfiguration<DnsObj>
{
    public void Configure(EntityTypeBuilder<DnsObj> builder)
    {
        //builder.ToTable("Dnses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name);
        builder.Property(x => x.FirstDns);
        builder.Property(x => x.SecondDns);
        builder.Property(x => x.CreationDate);
        builder.Property(x => x.ModifiedDate);
    }
}