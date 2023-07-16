﻿using _0_Framework.Application;
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
        OperationResult operationResult = new();
        if (_dnsObjRepository.Exists(x => x.Name == command.Name))
            return operationResult.Failed(ApplicationMessages.DuplicatedRecord);
        var dnsObj = new DnsObj(command.DnsAddresses, command.Name);
        _dnsObjRepository.Create(dnsObj);
        _dnsObjRepository.Save();
        return operationResult.Succeeded();
    }

    public OperationResult Delete(int id)
    {
        OperationResult operationResult = new();
        if (!_dnsObjRepository.Exists(x => x.Id == id)) return operationResult.Failed("TODO");
        _dnsObjRepository.Delete(id);
        _dnsObjRepository.Save();
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
        editDnsObj.Edit(command.DnsAddresses, command.Name);
        _dnsObjRepository.Save();
        return operationResult.Succeeded();
    }

    public List<DnsObjViewModel> GetAll()
    {
        return _dnsObjRepository.GetAll();
    }

    public OperationResult SetDns(int id)
    {
        OperationResult operationResult = new();
        var dnsObj = _dnsObjRepository.FindBy(id);
        try
        {
            dnsObj.SetDns();
            return operationResult.Succeeded();
        }
        catch (Exception e)
        {
            return operationResult.Failed("The Dns Index you Requested Does Not Exists");
        }
    }

    public OperationResult UnSetDns()
    {
        OperationResult operationResult = new();
        var dnsObj = new DnsObj();
        try
        {
            dnsObj.UnSetDns();
            return operationResult.Succeeded();
        }
        catch (Exception e)
        {
            return operationResult.Failed("TODO");
        }
    }

    public DnsObjViewModel GetCurrentDns()
    {
        DnsObjViewModel dnsObjViewModel = new();
        DnsObj dnsObj = new();
        dnsObjViewModel.DnsAddresses = dnsObj.GetCurrentDns();
        if (_dnsObjRepository.Exists(x => x.DnsAddresses == dnsObjViewModel.DnsAddresses))
            dnsObjViewModel = _dnsObjRepository.FindBy(dnsObjViewModel.DnsAddresses);
        return dnsObjViewModel;
    }
}