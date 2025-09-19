using SQLite;

namespace QD.Core.Domain.DnsObj;

[Table("Dns")]
public class DnsObj
{
    [PrimaryKey,AutoIncrement]
    public int Id { get; set; }
    public string FirstDns { get; set; }
    public string SecondDns { get; set; }
    public string Name { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime ModifiedDate { get; set; }

    public DnsObj()
    {
        
    }

    public DnsObj(string firstDns,string secondDns, string name)
    {
        FirstDns = firstDns;
        SecondDns = secondDns;
        Name = name;
        FirstDns = firstDns;
        CreationDate = DateTime.Now;
        ModifiedDate = CreationDate;
    }

    public void Edit(string firstDns,string secondDns, string name)
    {
        FirstDns = firstDns;
        SecondDns = secondDns;
        Name = name;
        ModifiedDate = DateTime.Now;
    }

}