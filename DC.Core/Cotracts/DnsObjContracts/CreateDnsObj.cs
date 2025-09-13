namespace DC.Core.Cotracts.DnsObjContracts;

public class CreateDnsObj
{
    public string DnsAddresses { get; set; }
    public string Name { get; set; }

    public CreateDnsObj()
    {
        
    }
    public CreateDnsObj(string dnsAddresses, string name)
    {
        DnsAddresses = dnsAddresses;
        Name = name;
    }
}