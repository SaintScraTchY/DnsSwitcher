﻿using HelperClass.Application;

namespace DC.Application.Contracts.DnsObjContracts;

public interface IDnsObjApplication
{
    OperationResult Create(CreateDnsObj command);
    OperationResult Delete(int id);
    EditDnsObj GetDetail(int id);
    OperationResult Edit(EditDnsObj command);
    List<DnsObjViewModel> GetAll();
    OperationResult SetDns(int id);
    OperationResult UnSetDns();
    DnsObjViewModel GetCurrentDns();
}