using System.Diagnostics.CodeAnalysis;
using System.Management;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.Marshalling;

namespace DC.Application.NetworkInterfaceHelper;
//[GeneratedComClass]
public partial class NetworkInterfaceClass
{
    public ManagementBaseObject ManagementBaseObject { get; init; }
    public ManagementObject InstanceManagementObject { get; init; }
    public NetworkInterface NetworkInterfaceObject { get; init; }
    [RequiresUnreferencedCode("Get ManagementObjects")]
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
    [RequiresUnreferencedCode("using ManagementObjects")]
    public void SetDns(string DnsAddresses)
    {
        var DnsArr = DnsAddresses.Split(',');
        ManagementBaseObject["DNSServerSearchOrder"] = DnsArr;
        InstanceManagementObject
            .InvokeMethod("SetDNSServerSearchOrder", ManagementBaseObject, null);
    }
    [RequiresUnreferencedCode("using ManagementObjects")]
    public void UnSetDns()
    {
        ManagementBaseObject["DNSServerSearchOrder"] = null;
        InstanceManagementObject
            .InvokeMethod("SetDNSServerSearchOrder", ManagementBaseObject, null);
    }
    [RequiresUnreferencedCode("using ManagementObjects")]
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
}