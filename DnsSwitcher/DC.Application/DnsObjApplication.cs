using System.Net.NetworkInformation;
using _0_Framework.Application;
using DC.Application.Contracts.DnsObjContracts;
using DC.Application.NetworkInterfaceHelper;
using DC.Domain.DnsObj;
using HelperClass.Application;

namespace DC.Application;

public class DnsObjApplication : IDnsObjApplication
{
    private readonly IDnsObjRepository _dnsObjRepository;
    private readonly NetworkInterfaceClass _networkInterface;

    public DnsObjApplication(IDnsObjRepository dnsObjRepository,NetworkInterfaceClass networkInterfaceClass)
    {
        _dnsObjRepository = dnsObjRepository;
        _networkInterface = networkInterfaceClass;
    }

    public async Task<OperationResult> Create(CreateDnsObj command)
    {
        OperationResult operationResult = new();
        if (await _dnsObjRepository.Exists(x => x.Name == command.Name || x.DnsAddresses == command.DnsAddresses))
            return operationResult.Failed(ApplicationMessages.DuplicatedRecord);
        var dnsObj = new DnsObj(command.DnsAddresses, command.Name);
        await _dnsObjRepository.CreateAsync(dnsObj);
        return operationResult.Succeeded();
    }

    public async Task<OperationResult> Delete(int id)
    {
        OperationResult operationResult = new();
        if (await _dnsObjRepository.Exists(x => x.Id != id))
            return operationResult.Failed(ApplicationMessages.RequestedRecordNotExists);
        await _dnsObjRepository.DeleteAsync(id);
        return operationResult.Succeeded();
    }

    public async Task<EditDnsObj> GetDetail(int id)
    {
        return await _dnsObjRepository.GetDetailAsync(id);
    }

    public async Task<OperationResult> Edit(EditDnsObj command)
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

    public async Task<List<DnsObjViewModel>> GetAll()
    {
        return await _dnsObjRepository.GetAllAsync();
    }

    public async Task<OperationResult> SetDns(int id)
    {
        OperationResult operationResult = new();
        var dnsObj = await _dnsObjRepository.FindByAsync(id);
        //dnsObj.SetDns();
        return operationResult.Succeeded();
        // try
        // {
        // }
        // catch (Exception e)
        // {
        //     return operationResult.Failed(ApplicationMessages.DnsChangeFailed);
        // }
    }

    public async Task<OperationResult> UnSetDns()
    {
        OperationResult operationResult = new();
        var dnsObj = new DnsObj();
        //dnsObj.UnSetDns();
        return operationResult.Succeeded();
        // try
        // {
        // }
        // catch (Exception e)
        // {
        //     return operationResult.Failed(ApplicationMessages.DnsUnSetFailed);
        // }
    }

    public async Task<DnsObjViewModel> GetCurrentDns()
    {
        DnsObjViewModel dnsObjViewModel = new();
        DnsObj dnsObj = new();
        dnsObjViewModel.DnsAddresses = _networkInterface.GetDns();
        if (await _dnsObjRepository.Exists(x => x.DnsAddresses == dnsObjViewModel.DnsAddresses))
            dnsObjViewModel = await _dnsObjRepository.FindByAsync(dnsObjViewModel.DnsAddresses);
        return dnsObjViewModel;
    }
    
    
}