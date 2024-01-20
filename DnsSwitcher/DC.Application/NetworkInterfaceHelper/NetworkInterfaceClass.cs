using System.Management;
using System.Net.NetworkInformation;

namespace DC.Application.NetworkInterfaceHelper;

public class NetworkInterfaceClass
{
    public ManagementBaseObject ManagementBaseObject { get; init; }
    public ManagementObject InstanceManagementObject { get; init; }
    public NetworkInterface NetworkInterfaceObject { get; init; }
    public NetworkInterfaceClass()
    {
        //Get Active Network Interface
        NetworkInterfaceObject = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault(x =>
            x.OperationalStatus == OperationalStatus.Up &&
            (x.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
             x.NetworkInterfaceType == NetworkInterfaceType.Ethernet) &&
            x.GetIPProperties().GatewayAddresses.Any(g => g.Address.AddressFamily.ToString() == "InterNetwork"));
        
        //Get The Correct NetworkInterFace
        if (NetworkInterfaceObject == null)
            Console.WriteLine("Turn Off your VPN");
        foreach (ManagementObject instance in new ManagementClass("Win32_NetworkAdapterConfiguration").GetInstances())
            if ((bool)instance["IPEnabled"] && instance["Description"].ToString().Equals(NetworkInterfaceObject.Description))
            {
                var methodParameters = instance.GetMethodParameters("SetDNSServerSearchOrder");
                if (methodParameters != null)
                {
                    ManagementBaseObject = methodParameters;
                    InstanceManagementObject = instance;
                }
            }
    }
    
    public void SetDns(string DnsAddresses)
    {
        var DnsArr = DnsAddresses.Split(',');
        ManagementBaseObject["DNSServerSearchOrder"] = DnsArr;
        InstanceManagementObject
            .InvokeMethod("SetDNSServerSearchOrder", ManagementBaseObject, null);
    }

    public void UnSetDns()
    {
        ManagementBaseObject["DNSServerSearchOrder"] = null;
        InstanceManagementObject
            .InvokeMethod("SetDNSServerSearchOrder", ManagementBaseObject, null);
    }

    public string GetDns()
    {
        var ipProperties = NetworkInterfaceObject.GetIPProperties();
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
        return GetDns();
    }
}