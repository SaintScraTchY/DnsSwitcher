namespace QD.Core.Cotracts.DnsObjContracts;

public class CreateDnsObj
{
    public string FirstDns { get; set; }
    public string SecondDns { get; set; }
    public string Name { get; set; }

    public CreateDnsObj()
    {
        
    }
    public CreateDnsObj(string firstDns,string secondDns, string name)
    {
        FirstDns = firstDns;
        SecondDns = secondDns;
        Name = name;
    }
}