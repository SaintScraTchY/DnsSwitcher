using DC.Core.Helper;

namespace DC.Core.Cotracts.DnsObjContracts;

public interface IDnsObjApplication
{
    Task<OperationResult> CreateAsync(CreateDnsObj command);
    Task<OperationResult> DeleteAsync(int id);
    Task<EditDnsObj> GetDetailAsync(int id);
    Task<OperationResult> EditAsync(EditDnsObj command);
    Task<List<DnsObjViewModel>> GetAllAsync();
    Task<OperationResult> SetDnsAsync(int id);
    Task<OperationResult> UnSetDnsAsync();
    Task<DnsObjViewModel> GetCurrentDnsAsync();
    OperationResult Create(CreateDnsObj command);
    OperationResult Delete(int id);
    EditDnsObj GetDetail(int id);
    OperationResult Edit(EditDnsObj command);
    IList<DnsObjViewModel> GetAll();
    OperationResult SetDns(int id);
    OperationResult UnSetDns();
    DnsObjViewModel GetCurrentDns();
}