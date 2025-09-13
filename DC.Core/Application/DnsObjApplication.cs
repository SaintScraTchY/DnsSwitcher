using DC.Core.Application.NetworkInterfaceHelper;
using DC.Core.Cotracts.DnsObjContracts;
using DC.Core.Domain.DnsObj;
using DC.Core.Helper;

namespace DC.Core.Application;

public class DnsObjApplication : IDnsObjApplication
{
    private readonly IDnsObjRepository _dnsObjRepository;
    private readonly NetworkInterfaceClass _networkInterface;

    public DnsObjApplication(IDnsObjRepository dnsObjRepository,NetworkInterfaceClass networkInterfaceClass)
    {
        _dnsObjRepository = dnsObjRepository;
        _networkInterface = networkInterfaceClass;
    }

    public async Task<OperationResult> CreateAsync(CreateDnsObj command)
    {
        OperationResult operationResult = new();
        if (await _dnsObjRepository.Exists(x => x.Name == command.Name || x.DnsAddresses == command.DnsAddresses))
            return operationResult.Failed(ApplicationMessages.DuplicatedRecord);
        var dnsObj = new DnsObj(command.DnsAddresses, command.Name);
        await _dnsObjRepository.CreateAsync(dnsObj);
        return operationResult.Succeeded();
    }

    public async Task<OperationResult> DeleteAsync(int id)
    {
        OperationResult operationResult = new();
        if (await _dnsObjRepository.Exists(x => x.Id != id))
            return operationResult.Failed(ApplicationMessages.RequestedRecordNotExists);
        await _dnsObjRepository.DeleteAsync(id);
        return operationResult.Succeeded();
    }

    public async Task<EditDnsObj> GetDetailAsync(int id)
    {
        return await _dnsObjRepository.GetDetailAsync(id);
    }

    public async Task<OperationResult> EditAsync(EditDnsObj command)
    {
        OperationResult operationResult = new();
        if (await _dnsObjRepository.Exists(x => x.Name == command.Name && x.Id != command.Id))
            return operationResult.Failed(ApplicationMessages.DuplicatedRecord);
        var editDnsObj = await _dnsObjRepository.FindByAsync(command.Id);
        editDnsObj.Edit(command.DnsAddresses, command.Name);
        if(await _dnsObjRepository.UpdateAsync(editDnsObj))
            return operationResult.Succeeded();
        //TODO
        return operationResult.Failed(ApplicationMessages.RequestedRecordNotExists);
    }

    public async Task<List<DnsObjViewModel>> GetAllAsync()
    {
        return await _dnsObjRepository.GetAllAsync();
    }

    public async Task<OperationResult> SetDnsAsync(int id)
    {
        OperationResult operationResult = new();
        var dnsObj = await _dnsObjRepository.FindByAsync(id);
        _networkInterface.SetDns(dnsObj.DnsAddresses);
        return operationResult.Succeeded();
        // try
        // {
        // }
        // catch (Exception e)
        // {
        //     return operationResult.Failed(ApplicationMessages.DnsChangeFailed);
        // }
    }

    public async Task<OperationResult> UnSetDnsAsync()
    {
        OperationResult operationResult = new();
        _networkInterface.UnSetDns();
        return operationResult.Succeeded();
        // try
        // {
        // }
        // catch (Exception e)
        // {
        //     return operationResult.Failed(ApplicationMessages.DnsUnSetFailed);
        // }
    }

    public async Task<DnsObjViewModel> GetCurrentDnsAsync()
    {
        DnsObjViewModel dnsObjViewModel = new();
        dnsObjViewModel.DnsAddresses = _networkInterface.GetDns();
        if (await _dnsObjRepository.Exists(x => x.DnsAddresses == dnsObjViewModel.DnsAddresses))
            dnsObjViewModel = await _dnsObjRepository.FindByAsync(dnsObjViewModel.DnsAddresses);
        return dnsObjViewModel;
    }

    public OperationResult Create(CreateDnsObj command)
    {
        throw new NotImplementedException();
    }

    public OperationResult Delete(int id)
    {
        throw new NotImplementedException();
    }

    public EditDnsObj GetDetail(int id)
    {
        throw new NotImplementedException();
    }

    public OperationResult Edit(EditDnsObj command)
    {
        throw new NotImplementedException();
    }

    public IList<DnsObjViewModel> GetAll()
    {
        throw new NotImplementedException();
    }

    public OperationResult SetDns(int id)
    {
        throw new NotImplementedException();
    }

    public OperationResult UnSetDns()
    {
        throw new NotImplementedException();
    }

    public DnsObjViewModel GetCurrentDns()
    {
        throw new NotImplementedException();
    }
}