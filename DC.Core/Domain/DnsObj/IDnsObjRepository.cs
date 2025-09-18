using System.Linq.Expressions;
using DC.Core.Cotracts.DnsObjContracts;

namespace DC.Core.Domain.DnsObj;

public interface IDnsObjRepository
{
    Task<bool> CreateAsync(DnsObj entity);
    Task<bool> UpdateAsync(DnsObj entity);
    Task<List<DnsObjViewModel>> GetAllAsync();
    Task<bool> DeleteAsync(int id);
    Task<DnsObj> FindByAsync(int id);
    Task<DnsObjViewModel> FindByAsync(string dns);
    Task<EditDnsObj> GetDetailAsync(int id);
    Task<bool> ExistsAsync(Expression<Func<DnsObj, bool>> expression);
    Task<DnsObjViewModel> FindByAsync(Expression<Func<DnsObj, bool>> expression);
    Task<bool> IsDuplicateDnsAsync(string name ,string firstDns, string secondDns);
    
    
    bool Create(DnsObj entity);
    bool Update(DnsObj entity);
    List<DnsObjViewModel> GetAll();
    bool Delete(int id);
    DnsObj FindBy(int id);
    DnsObjViewModel FindBy(string dns);
    EditDnsObj GetDetail(int id);
    bool Exists(Expression<Func<DnsObj, bool>> expression);
    DnsObjViewModel FindBy(Expression<Func<DnsObj, bool>> expression);
    bool IsDuplicateDns(string name ,string firstDns, string secondDns);
}