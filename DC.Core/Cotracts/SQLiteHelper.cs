using DC.Core.Cotracts.DnsObjContracts;
using DC.Core.Domain.DnsObj;

namespace DC.Core.Cotracts;

public static class SQLiteHelper
{
    public static DnsObjViewModel MapDnsObjViewModel(this DnsObj dnsObj)
    {
        return new DnsObjViewModel()
        {
            Id = dnsObj.Id,
            DnsAddresses = dnsObj.FirstDns + " | " + dnsObj.SecondDns,
            Name = dnsObj.Name,
        };
    }

    public static EditDnsObj MapEditDnsObj(this DnsObj dnsObj)
    {
        return new EditDnsObj
        {
            Id = dnsObj.Id,
            Name = dnsObj.Name,
            FirstDns = dnsObj.FirstDns,
            SecondDns = dnsObj.SecondDns,
        };
    }
}