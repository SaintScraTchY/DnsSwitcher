using System.Management;
using System.Net.NetworkInformation;

namespace DC.Domain.DnsObj;

public class DnsObj
{
    public DnsObj()
    {
    }

    public DnsObj(string dnsAddresses, string name)
    {
        DnsAddresses = dnsAddresses;
        Name = name;
    }

    public int Id { get; private set; }
    public string DnsAddresses { get; private set; }
    public string Name { get; private set; }

    public void Edit(string dnsAddresses, string name)
    {
        DnsAddresses = dnsAddresses;
        Name = name;
    }

    public NetworkInterface GetActiveNic()
    {
        var Nic = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(x =>
            x.OperationalStatus == OperationalStatus.Up &&
            (x.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
             x.NetworkInterfaceType == NetworkInterfaceType.Ethernet) &&
            x.GetIPProperties().GatewayAddresses.Any(g => g.Address.AddressFamily.ToString() == "InterNetwork"));
        return Nic;
    }

    public NIC GetCorrectNIC()
    {
        var networkInterface = GetActiveNic();
        if (networkInterface == null)
            Console.WriteLine("Turn Off your VPN");
        var nic = new NIC();
        foreach (ManagementObject instance in new ManagementClass("Win32_NetworkAdapterConfiguration").GetInstances())
            if ((bool) instance["IPEnabled"] && instance["Description"].ToString().Equals(networkInterface.Description))
            {
                var methodParameters = instance.GetMethodParameters("SetDNSServerSearchOrder");
                if (methodParameters != null)
                {
                    nic = new NIC(methodParameters, instance);
                    return nic;
                }
            }
        return nic;
    }

    public void SetDns()
    {
        var NetworkInterface = GetCorrectNIC();
        var DnsArr = DnsAddresses.Split(',');
        NetworkInterface.ManagementBaseObject["DNSServerSearchOrder"] = DnsArr;
        NetworkInterface.InstanceManagementObject
            .InvokeMethod("SetDNSServerSearchOrder", NetworkInterface.ManagementBaseObject, null);
    }

    public void UnSetDns()
    {
        var NetworkInterface = GetCorrectNIC();
        NetworkInterface.ManagementBaseObject["DNSServerSearchOrder"] = null;
        NetworkInterface.InstanceManagementObject
            .InvokeMethod("SetDNSServerSearchOrder", NetworkInterface.ManagementBaseObject, null);
    }

    public string GetDns(NetworkInterface networkInterface)
    {
         var ipProperties = networkInterface.GetIPProperties();
         var dnsCollection = ipProperties.DnsAddresses;
         var dns = "";
         foreach (var dnsAddress in dnsCollection)
         {
             dns += dnsAddress;
             if (dnsCollection.First() == dnsAddress)
                 dns += ",";
         }
        return dns;
    }

    public string GetCurrentDns()
    {
        return GetDns(GetActiveNic());
    }
}