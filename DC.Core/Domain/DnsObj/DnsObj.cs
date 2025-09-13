using SQLite;

namespace DC.Core.Domain.DnsObj;

[Table("Dns")]
public class DnsObj
{
    [PrimaryKey,AutoIncrement]
    public int Id { get; set; }
    public string DnsAddresses { get; set; }
    public string Name { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ModifiedDate { get; set; }

    public DnsObj()
    {
    }

    public DnsObj(string dnsAddresses, string name)
    {
        DnsAddresses = dnsAddresses;
        Name = name;
        CreationDate = DateTime.Now;
        ModifiedDate = CreationDate;
    }

    public void Edit(string dnsAddresses, string name)
    {
        DnsAddresses = dnsAddresses;
        Name = name;
        ModifiedDate = DateTime.Now;
    }

}