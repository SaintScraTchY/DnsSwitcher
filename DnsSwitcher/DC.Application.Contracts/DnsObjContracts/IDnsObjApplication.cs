using HelperClass.Application;

namespace DC.Application.Contracts.DnsObjContracts;

public interface IDnsObjApplication
{
    Task<OperationResult> Create(CreateDnsObj command);
    Task<OperationResult> Delete(int id);
    Task<EditDnsObj> GetDetail(int id);
    Task<OperationResult> Edit(EditDnsObj command);
    Task<List<DnsObjViewModel>> GetAll();
    Task<OperationResult> SetDns(int id);
    Task<OperationResult> UnSetDns();
    Task<DnsObjViewModel> GetCurrentDns();
}