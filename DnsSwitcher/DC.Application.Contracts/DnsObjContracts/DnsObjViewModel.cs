namespace DC.Application.Contracts.DnsObjContracts;

public class DnsObjViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DnsAddresses { get; set; }

    public DnsObjViewModel()
    {
        
    }
    
    public DnsObjViewModel(int id, string name, string dnsAddresses)
    {
        Id = id;
        Name = name;
        DnsAddresses = dnsAddresses;
    }
}