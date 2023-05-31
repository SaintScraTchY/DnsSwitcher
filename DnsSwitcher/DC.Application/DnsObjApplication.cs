using _0_Framework.Application;
using DC.Application.Contracts.DnsObjContracts;
using DC.Domain.DnsObj;
using HelperClass.Application;

namespace DC.Application;

public class DnsObjApplication : IDnsObjApplication
{
    private readonly IDnsObjRepository _dnsObjRepository;

    public DnsObjApplication(IDnsObjRepository dnsObjRepository)
    {
        _dnsObjRepository = dnsObjRepository;
    }

    public OperationResult Create(CreateDnsObj command)
    {
        OperationResult operationResult =new();
        if (_dnsObjRepository.Exists(x=>x.Name == command.Name))
            return operationResult.Failed(ApplicationMessages.DuplicatedRecord);
        DnsObj dnsObj = new DnsObj(command.DnsAddresses, command.Name);
        _dnsObjRepository.Create(dnsObj);
        return operationResult.Succeeded();
    }

    public OperationResult Edit(EditDnsObj command)
    {
        OperationResult operationResult =new();
        if (_dnsObjRepository.Exists(x=>x.Name ==command.Name && x.Id != command.Id))
            return operationResult.Failed(ApplicationMessages.DuplicatedRecord);
        return operationResult.Succeeded();
    }

    public List<DnsObjViewModel> GetAll()
    {
        throw new NotImplementedException();
    }

    public OperationResult SetDns(int id)
    {
        OperationResult operationResult = new();
        DnsObj dnsObj = _dnsObjRepository.FindBy(id);
        try
        {
            dnsObj.SetDns();
            return operationResult.Succeeded();
        }
        catch (Exception e)
        {
            return operationResult.Failed("Failed");
        }
    }

    public OperationResult UnSetDns()
    {
        OperationResult operationResult = new();
        DnsObj dnsObj = new DnsObj();
        try
        {
            dnsObj.UnSetDns();
            return operationResult.Succeeded();
        }
        catch (Exception e)
        {
            return operationResult.Failed("failed");
        }
    }
}