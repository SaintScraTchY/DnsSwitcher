using System.Linq.Expressions;
using DC.Core.Cotracts.DnsObjContracts;
using DC.Core.Domain.DnsObj;

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
            var toBeAdd = new DnsObjViewModel(dnsObj.Id, dnsObj.Name, dnsObj.FirstDns,dnsObj.SecondDns);
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
        DnsObj entity = await _context.Database.Table<DnsObj>().FirstOrDefaultAsync(x=>x.FirstDns == dns || x.SecondDns == dns);
        return new DnsObjViewModel(entity.Id, entity.Name, entity.FirstDns, entity.SecondDns);
    }

    public async Task<EditDnsObj> GetDetailAsync(int id)
    {
        await _context.InitAsync();
        DnsObj entity = await FindByAsync(id);
        return new EditDnsObj(entity.FirstDns,entity.SecondDns, entity.Name, entity.Id);
    }

    public async Task<bool> ExistsAsync(Expression<Func<DnsObj, bool>> expression)
    {
        await _context.InitAsync();
        var result = await _context.Database.Table<DnsObj>().FirstOrDefaultAsync(expression);
        
        if (result is null)
            return false;

        return true;
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
}