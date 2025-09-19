namespace QD.Core.Cotracts.DnsObjContracts;

public class EditDnsObj : CreateDnsObj
{
    public int Id { get; set; }

    public EditDnsObj()
    {
        
    }
    public EditDnsObj(string firstDns,string secondDns, string name, int id) : base(firstDns,secondDns, name)
    {
        Id = id;
    }
}