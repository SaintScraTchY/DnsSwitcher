using QD.Core.Application.NetworkInterfaceHelper;
using QD.Core.Cotracts.DnsObjContracts;
using QD.Core.Domain.DnsObj;
using QD.Core.Helper;

namespace QD.Core.Application;

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
        if (_dnsObjRepository.IsDuplicateDns(command.Name,command.FirstDns,command.SecondDns))
            return operationResult.Failed(ApplicationMessages.DuplicatedRecord);
        var dnsObj = new DnsObj(command.FirstDns,command.SecondDns, command.Name);
        await _dnsObjRepository.CreateAsync(dnsObj);
        return operationResult.Succeeded();
    }

    public async Task<OperationResult> DeleteAsync(int id)
    {
        OperationResult operationResult = new();
        if (await _dnsObjRepository.ExistsAsync(x => x.Id != id))
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
        if (await _dnsObjRepository.ExistsAsync(x => x.Name == command.Name && x.Id != command.Id))
            return operationResult.Failed(ApplicationMessages.DuplicatedRecord);
        var editDnsObj = await _dnsObjRepository.FindByAsync(command.Id);
        editDnsObj.Edit(command.FirstDns,command.SecondDns, command.Name);
        if(await _dnsObjRepository.UpdateAsync(editDnsObj))
            return operationResult.Succeeded();
        
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
        _networkInterface.SetDns(dnsObj.FirstDns,dnsObj.SecondDns);
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
        var dnsArr = _networkInterface.GetDns();
        if (await _dnsObjRepository.ExistsAsync(x => (x.FirstDns == dnsArr.FirstDns && x.SecondDns == dnsArr.SecondDns) 
                                                     || (x.FirstDns == dnsArr.SecondDns && x.SecondDns == dnsArr.FirstDns)))
            dnsObjViewModel = await _dnsObjRepository.FindByAsync(dnsObjViewModel.DnsAddresses);
        return dnsObjViewModel;
    }

    public OperationResult Create(CreateDnsObj command)
    {
        OperationResult operationResult = new();
        if (_dnsObjRepository.IsDuplicateDns(command.Name,command.FirstDns,command.SecondDns))
            return operationResult.Failed(ApplicationMessages.DuplicatedRecord);
        var dnsObj = new DnsObj(command.FirstDns,command.SecondDns, command.Name);
        _dnsObjRepository.Create(dnsObj);
        return operationResult.Succeeded();
    }

    public OperationResult Delete(int id)
    {
        OperationResult operationResult = new();
        if (_dnsObjRepository.Exists(x => x.Id != id))
            return operationResult.Failed(ApplicationMessages.RequestedRecordNotExists);
        _dnsObjRepository.Delete(id);
        return operationResult.Succeeded();
    }

    public EditDnsObj GetDetail(int id)
    {
        return _dnsObjRepository.GetDetail(id);
    }

    public OperationResult Edit(EditDnsObj command)
    {
        OperationResult operationResult = new();
        if (_dnsObjRepository.Exists(x => x.Name == command.Name && x.Id != command.Id))
            return operationResult.Failed(ApplicationMessages.DuplicatedRecord);
        var editDnsObj = _dnsObjRepository.FindBy(command.Id);
        editDnsObj.Edit(command.FirstDns,command.SecondDns, command.Name);
        if(_dnsObjRepository.Update(editDnsObj))
            return operationResult.Succeeded();
        
        return operationResult.Failed(ApplicationMessages.RequestedRecordNotExists);
    }

    public IList<DnsObjViewModel> GetAll()
    {
        return _dnsObjRepository.GetAll();
    }

    public OperationResult SetDns(int id)
    {
        OperationResult operationResult = new();
        var dnsObj = _dnsObjRepository.FindBy(id);
        _networkInterface.SetDns(dnsObj.FirstDns,dnsObj.SecondDns);
        return operationResult.Succeeded();
    }

    public OperationResult UnSetDns()
    {
        OperationResult operationResult = new();
        _networkInterface.UnSetDns();
        return operationResult.Succeeded();
    }

    public DnsObjViewModel GetCurrentDns()
    {
        DnsObjViewModel dnsObjViewModel = new();
        var dnsArr = _networkInterface.GetDns();
        if (_dnsObjRepository.Exists(x => (x.FirstDns == dnsArr.FirstDns && x.SecondDns == dnsArr.SecondDns) 
                                                     || (x.FirstDns == dnsArr.SecondDns && x.SecondDns == dnsArr.FirstDns)))
            dnsObjViewModel = _dnsObjRepository.FindBy(dnsObjViewModel.DnsAddresses);
        return dnsObjViewModel;
    }
}