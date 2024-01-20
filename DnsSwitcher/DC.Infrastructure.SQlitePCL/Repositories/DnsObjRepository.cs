using System.Linq.Expressions;
using DC.Application.Contracts.DnsObjContracts;
using DC.Domain.DnsObj;

namespace DC.Infrastructure.SQlitePCL.Repositories;

public class DnsObjRepository : IDnsObjRepository
{
    private readonly DnsContext _context;

    public DnsObjRepository(DnsContext context)
    {
        _context = context;
        _context.InitAsync();
    }

    public async Task<bool> CreateAsync(DnsObj entity)
    {
        await _context.InitAsync();
        return await _context.Database.InsertAsync(entity) > 0;
    }

    public async Task<bool> UpdateAsync(DnsObj entity)
    {
        await _context.InitAsync();
        return await _context.Database.UpdateAsync(entity) > 0;
    }

    public async Task<List<DnsObjViewModel>> GetAllAsync()
    {
        await _context.InitAsync();
        var rawList = await _context.Database.Table<DnsObj>().ToListAsync();
        List<DnsObjViewModel> dnsList = new();
        foreach (var dnsObj in rawList)
        {
            var toBeAdd = new DnsObjViewModel(dnsObj.Id, dnsObj.Name, dnsObj.DnsAddresses);
            dnsList.Add(toBeAdd);
        }
        return dnsList;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await _context.InitAsync();
        var entity = FindByAsync(id);
        return await _context.Database.DeleteAsync(entity) > 0;
    }

    public async Task<DnsObj> FindByAsync(int id)
    {
        await _context.InitAsync();
        return await _context.Database.GetAsync<DnsObj>(id);
    }

    public async Task<DnsObjViewModel> FindByAsync(string dns)
    {
        await _context.InitAsync();
        DnsObj entity = await _context.Database.Table<DnsObj>().FirstOrDefaultAsync(x=>x.DnsAddresses == dns);
        return new DnsObjViewModel(entity.Id, entity.Name, entity.DnsAddresses);
    }

    public async Task<EditDnsObj> GetDetailAsync(int id)
    {
        await _context.InitAsync();
        DnsObj entity = await FindByAsync(id);
        return new EditDnsObj(entity.DnsAddresses, entity.Name, entity.Id);
    }

    public async Task<bool> Exists(Expression<Func<DnsObj, bool>> expression)
    {
        await _context.InitAsync();
        var result = await _context.Database.Table<DnsObj>().FirstOrDefaultAsync(expression);
        
        if (result is null)
            return false;

        return true;
    }
}