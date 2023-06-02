using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using HelperClass.Application;

namespace DC.Domain.DnsObj;

public class DnsObj
{
    public int Id { get; private set; }
    public string DnsAddresses { get; private set; }
    public string Name { get; private set; }

    public DnsObj()
    {
        
    }
    public DnsObj(string dnsAddresses, string name)
    {
        DnsAddresses = dnsAddresses;
        Name = name;
    }

    public void Edit(string dnsAddresses, string name)
    {
        DnsAddresses = dnsAddresses;
        Name = name;
    }
    
    public NetworkInterface GetActiveNic()
    {
        NetworkInterface? Nic = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(x =>
            x.OperationalStatus == OperationalStatus.Up &&
            (x.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
             x.NetworkInterfaceType == NetworkInterfaceType.Ethernet) &&
            x.GetIPProperties().GatewayAddresses.Any(g => g.Address.AddressFamily.ToString() == "InterNetwork"));
        return Nic;
    }

    public void SetDns()
    {
        NetworkInterface networkInterface = GetActiveNic();
        if(networkInterface == null)
            Console.WriteLine("Turn Off your VPN");
            
        foreach (ManagementObject instance in new ManagementClass("Win32_NetworkAdapterConfiguration").GetInstances())
        {
            if ((bool)instance["IPEnabled"] && instance["Description"].ToString().Equals(networkInterface.Description))
            {
                ManagementBaseObject methodParameters = instance.GetMethodParameters("SetDNSServerSearchOrder");
                if (methodParameters != null)
                {
                    string[] DnsArr = DnsAddresses.Split(',');
                    methodParameters["DNSServerSearchOrder"] = DnsArr;
                    instance.InvokeMethod("SetDNSServerSearchOrder", methodParameters, (InvokeMethodOptions)null);
                    Console.WriteLine("Dns has been Set");
                }
            }
        }
    }

    public void UnSetDns()
    {
        NetworkInterface networkInterface = GetActiveNic();
        if(networkInterface == null)
            Console.WriteLine("Turn Off your VPN");
            
        foreach (ManagementObject instance in new ManagementClass("Win32_NetworkAdapterConfiguration").GetInstances())
        {
            if ((bool)instance["IPEnabled"] && instance["Description"].ToString().Equals(networkInterface.Description))
            {
                ManagementBaseObject methodParameters = instance.GetMethodParameters("SetDNSServerSearchOrder");
                if (methodParameters != null)
                {
                    methodParameters["DNSServerSearchOrder"] = null;
                    instance.InvokeMethod("SetDNSServerSearchOrder", methodParameters, (InvokeMethodOptions)null);
                    Console.WriteLine("Dns has been Set");
                }
            }
        }
    }
}