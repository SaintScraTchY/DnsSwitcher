namespace DC.Core.Cotracts.DnsObjContracts;

public class EditDnsObj : CreateDnsObj
{
    public int Id { get; set; }

    public EditDnsObj()
    {
        
    }
    public EditDnsObj(string dnsAddresses, string name, int id) : base(dnsAddresses, name)
    {
        Id = id;
    }
}