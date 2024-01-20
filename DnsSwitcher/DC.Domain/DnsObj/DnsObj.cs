using System.Management;
using System.Net.NetworkInformation;
using SQLite;

namespace DC.Domain.DnsObj;

public class DnsObj
{
    public int Id { get; private set; }
    public string DnsAddresses { get; private set; }
    public string Name { get; private set; }
    public DateTime CreationDate { get; private set; }
    public DateTime ModifiedDate { get; private set; }

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