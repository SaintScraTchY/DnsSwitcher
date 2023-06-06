using System;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;

namespace DnsChangerConsole.Services;

public class DnsService
{
    public bool SetDns(string[] Dns)
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
                    try
                    {
                        methodParameters["DNSServerSearchOrder"] = (object)Dns;
                        instance.InvokeMethod("SetDNSServerSearchOrder", methodParameters, (InvokeMethodOptions)null);
                        Console.WriteLine("Dns has been Set");
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed");
                        return false;
                    }
                }
            }
        }

        Console.WriteLine("Nic Cannot be Found");
        return false;
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
}