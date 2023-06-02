using System.Linq.Expressions;
using DC.Application.Contracts.DnsObjContracts;
using HelperClass.Application;

namespace DC.Domain.DnsObj;

public interface IDnsObjRepository
{
    void Save();
    void Create(DnsObj entity);
    List<DnsObjViewModel> GetAll();
    void Delete(int id);
    DnsObj FindBy(int id);
    EditDnsObj GetDetail(int id);
    bool Exists(Expression<Func<DnsObj,bool>> expression);
}