using System.Linq.Expressions;
using DC.Application.Contracts.DnsObjContracts;
using DC.Domain.DnsObj;

namespace DC.Infrastructure.SQlite.Repositories;

public class DnsObjRepository : IDnsObjRepository
{
    private readonly DnsContext _context;

    public DnsObjRepository(DnsContext context)
    {
        _context = context;
    }

    public void Save()
    {
        _context.SaveChanges();
    }

    public void Create(DnsObj entity)
    {
        _context.Dnses.Add(entity);
    }

    public List<DnsObjViewModel> GetAll()
    {
        return _context.Dnses.Select(x => new DnsObjViewModel
        {
            Id = x.Id,
            Name = x.Name,
            DnsAddresses = x.DnsAddresses
        }).ToList();
    }

    public void Delete(int id)
    {
        _context.Dnses.Remove(FindBy(id));
    }

    public DnsObj FindBy(int id)
    {
        return _context.Dnses.FirstOrDefault(x => x.Id == id);
    }

    public DnsObjViewModel FindBy(string dns)
    {
        return _context.Dnses.Select(x => new DnsObjViewModel
        {
            Id = x.Id,
            Name = x.Name,
            DnsAddresses = x.DnsAddresses
        }).FirstOrDefault(x => x.DnsAddresses == dns);
    }

    public EditDnsObj GetDetail(int id)
    {
        return _context.Dnses.Select(x => new EditDnsObj
        {
            Id = x.Id,
            Name = x.Name,
            DnsAddresses = x.DnsAddresses
        }).FirstOrDefault(x => x.Id == id);
    }

    public bool Exists(Expression<Func<DnsObj, bool>> expression)
    {
        return _context.Dnses.Any(expression);
    }
}