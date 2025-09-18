using System.Linq.Expressions;
using DC.Core.Cotracts;
using DC.Core.Cotracts.DnsObjContracts;
using DC.Core.Domain.DnsObj;
using Microsoft.EntityFrameworkCore;

namespace DC.Core.Infrastructure.EFCore.Repositories;

public class DnsObjRepository : IDnsObjRepository
{
    private readonly DnsContext _context;

    public DnsObjRepository(DnsContext context)
    {
        _context = context;
    }

    private async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(DnsObj entity)
    {
        var result = _context.Dnses.Update(entity);
        await SaveAsync();
        return result.State == EntityState.Modified;
    }

    public async Task<bool> CreateAsync(DnsObj entity)
    {
        var result = await _context.Dnses.AddAsync(entity);
        await SaveAsync();
        return result.State == EntityState.Added;
    }

    public async Task<List<DnsObjViewModel>> GetAllAsync()
    {
        return await _context.Dnses.Select(x => x.MapDnsObjViewModel()).ToListAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
         var result =_context.Dnses.Remove(await FindByAsync(id));
         await SaveAsync();
         return result.State == EntityState.Deleted;
    }

    public async Task<DnsObj> FindByAsync(int id)
    {
        return await _context.Dnses.FirstOrDefaultAsync(x => x.Id == id) ?? new DnsObj();
    }

    public async Task<DnsObjViewModel> FindByAsync(string dns)
    {
        return await _context.Dnses.Select(x => x.MapDnsObjViewModel()).FirstOrDefaultAsync(x => x.DnsAddresses == dns) ?? new DnsObjViewModel();
    }

    public async Task<EditDnsObj> GetDetailAsync(int id)
    {
        return await _context.Dnses.Select(x => x.MapEditDnsObj()).FirstOrDefaultAsync(x => x.Id == id) ?? new EditDnsObj();
    }

    public Task<bool> ExistsAsync(Expression<Func<DnsObj, bool>> expression)
    {
        return _context.Dnses.AnyAsync(expression);
    }

    public Task<DnsObjViewModel> FindByAsync(Expression<Func<DnsObj, bool>> expression)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsDuplicateDnsAsync(string name, string firstDns, string secondDns)
    {
        throw new NotImplementedException();
    }

    public bool Create(DnsObj entity)
    {
        throw new NotImplementedException();
    }

    public bool Update(DnsObj entity)
    {
        throw new NotImplementedException();
    }

    public List<DnsObjViewModel> GetAll()
    {
        throw new NotImplementedException();
    }

    public bool Delete(int id)
    {
        throw new NotImplementedException();
    }

    public DnsObj FindBy(int id)
    {
        throw new NotImplementedException();
    }

    public DnsObjViewModel FindBy(string dns)
    {
        throw new NotImplementedException();
    }

    public EditDnsObj GetDetail(int id)
    {
        throw new NotImplementedException();
    }

    public bool Exists(Expression<Func<DnsObj, bool>> expression)
    {
        throw new NotImplementedException();
    }

    public DnsObjViewModel FindBy(Expression<Func<DnsObj, bool>> expression)
    {
        throw new NotImplementedException();
    }

    public bool IsDuplicateDns(string name, string firstDns, string secondDns)
    {
        throw new NotImplementedException();
    }
}