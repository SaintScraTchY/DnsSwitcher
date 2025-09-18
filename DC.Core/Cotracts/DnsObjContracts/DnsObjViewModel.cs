namespace DC.Core.Cotracts.DnsObjContracts;

public class DnsObjViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DnsAddresses { get; set; }
    public string FirstDns { get; set; }
    public string SecondDns { get; set; }

    public DnsObjViewModel()
    {
        
    }
    
    public DnsObjViewModel(int id, string name, string firstDns, string secondDns)
    {
        Id = id;
        Name = name;
        DnsAddresses = firstDns;
        FirstDns = secondDns;
        DnsAddresses = firstDns + " | " + secondDns;
    }
}