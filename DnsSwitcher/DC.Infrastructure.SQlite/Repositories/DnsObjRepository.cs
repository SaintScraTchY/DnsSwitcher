using System.Linq.Expressions;
using DC.Domain.DnsObj;

namespace DC.Infrastructure.SQlite.Repositories;

public class DnsObjRepository : IDnsObjRepository
{
    private readonly DnsContext _context;

    public DnsObjRepository(DnsContext context)
    {
        _context = context;
    }

    public void Create(DnsObj entity)
    {
        _context.DnsObjects.Add(entity);
    }

    public void Delete(int id)
    {
        _context.DnsObjects.Remove(FindBy(id));
    }

    public DnsObj FindBy(int id)
    {
        return _context.DnsObjects.FirstOrDefault(x => x.Id == id);
    }

    public bool Exists(Expression<Func<DnsObj, bool>> expression)
    {
        return _context.DnsObjects.Any(expression);
    }
}