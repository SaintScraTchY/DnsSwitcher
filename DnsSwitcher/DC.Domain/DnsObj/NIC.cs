using System.Management;

namespace DC.Domain.DnsObj;

public class NIC
{
    public NIC()
    {
    }

    public NIC(ManagementBaseObject managementBaseObject, ManagementObject instanceManagementObject)
    {
        ManagementBaseObject = managementBaseObject;
        InstanceManagementObject = instanceManagementObject;
    }

    public ManagementBaseObject ManagementBaseObject { get; }
    public ManagementObject InstanceManagementObject { get; }
}