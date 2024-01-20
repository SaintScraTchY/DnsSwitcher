using System.Linq.Expressions;
using DC.Application.Contracts.DnsObjContracts;

namespace DC.Domain.DnsObj;

public interface IDnsObjRepository
{
    Task<bool> CreateAsync(DnsObj entity);
    Task<bool> UpdateAsync(DnsObj entity);
    Task<List<DnsObjViewModel>> GetAllAsync();
    Task<bool> DeleteAsync(int id);
    Task<DnsObj> FindByAsync(int id);
    Task<DnsObjViewModel> FindByAsync(string dns);
    Task<EditDnsObj> GetDetailAsync(int id);
    Task<bool> Exists(Expression<Func<DnsObj, bool>> expression);
}