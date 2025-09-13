using System.Linq.Expressions;
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
        return await _context.Dnses.Select(x => new DnsObjViewModel
        {
            Id = x.Id,
            Name = x.Name,
            DnsAddresses = x.DnsAddresses
        }).ToListAsync();
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
        return await _context.Dnses.Select(x => new DnsObjViewModel
        {
            Id = x.Id,
            Name = x.Name,
            DnsAddresses = x.DnsAddresses
        }).FirstOrDefaultAsync(x => x.DnsAddresses == dns) ?? new DnsObjViewModel();
    }

    public async Task<EditDnsObj> GetDetailAsync(int id)
    {
        return await _context.Dnses.Select(x => new EditDnsObj
        {
            Id = x.Id,
            Name = x.Name,
            DnsAddresses = x.DnsAddresses
        }).FirstOrDefaultAsync(x => x.Id == id) ?? new EditDnsObj();
    }

    public Task<bool> Exists(Expression<Func<DnsObj, bool>> expression)
    {
        return _context.Dnses.AnyAsync(expression);
    }
}