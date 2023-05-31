using System.Linq.Expressions;
using HelperClass.Application;

namespace DC.Domain.DnsObj;

public interface IDnsObjRepository
{
    void Create(DnsObj entity);
    void Delete(int id);
    DnsObj FindBy(int id);
    bool Exists(Expression<Func<DnsObj,bool>> expression);
}