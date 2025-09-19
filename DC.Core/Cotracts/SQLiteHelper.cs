using DC.Core.Cotracts.DnsObjContracts;
using DC.Core.Domain.DnsObj;

namespace DC.Core.Cotracts;

public static class SQLiteHelper
{
    public static DnsObjViewModel MapDnsObjViewModel(this DnsObj dnsObj)
    {
        return new DnsObjViewModel(dnsObj.Id, dnsObj.Name, dnsObj.FirstDns, dnsObj.SecondDns);
    }

    public static EditDnsObj MapEditDnsObj(this DnsObj dnsObj)
    {
        return new EditDnsObj( dnsObj.Name, dnsObj.FirstDns, dnsObj.SecondDns, dnsObj.Id );
    }
    
    public static EditDnsObj MapEditDnsObj(this DnsObjViewModel dnsObj)
    {
        return new EditDnsObj( dnsObj.Name, dnsObj.FirstDns, dnsObj.SecondDns, dnsObj.Id );
    }
}