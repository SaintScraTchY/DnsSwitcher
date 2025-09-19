using System.Diagnostics.CodeAnalysis;
using System.Management;
using System.Net.NetworkInformation;
using QD.Core.Cotracts.DnsObjContracts;

namespace QD.Core.Application.NetworkInterfaceHelper;
//[GeneratedComClass]
public partial class NetworkInterfaceClass
{
    private ManagementBaseObject ManagementBaseObject { get; init; }
    private ManagementObject InstanceManagementObject { get; init; }
    private NetworkInterface NetworkInterfaceObject { get; init; }
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
    public void SetDns(string firstDns,string secondDns)
    {
        ManagementBaseObject["DNSServerSearchOrder"] = new string[] { firstDns, secondDns };
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
    public (string FirstDns,string SecondDns) GetDns()
    {
        var ipProperties = NetworkInterfaceObject.GetIPProperties();
        var dnsCollection = ipProperties.DnsAddresses;
        return dnsCollection.Count switch
        {
            0 => (string.Empty, string.Empty),
            1 => (dnsCollection[0].ToString(), string.Empty),
            _ => (dnsCollection[0].ToString(), dnsCollection[1].ToString())
        };
    }
}