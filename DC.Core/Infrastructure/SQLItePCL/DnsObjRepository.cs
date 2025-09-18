using System.Linq.Expressions;
using DC.Core.Cotracts;
using DC.Core.Cotracts.DnsObjContracts;
using DC.Core.Domain.DnsObj;

namespace DC.Core.Infrastructure.SQLItePCL;

public class DnsObjRepository : IDnsObjRepository
{
    private readonly DnsContext _context;

    public DnsObjRepository(DnsContext context)
    {
        _context = context;
        Task.Run(async () => await _context.InitAsync());
    }

    public async Task<bool> CreateAsync(DnsObj entity)
    {
        await _context.InitAsync();
        return await _context.AsyncDbConnection.InsertAsync(entity) > 0;
    }

    public async Task<bool> UpdateAsync(DnsObj entity)
    {
        await _context.InitAsync();
        return await _context.AsyncDbConnection.UpdateAsync(entity) > 0;
    }

    public async Task<List<DnsObjViewModel>> GetAllAsync()
    {
        await _context.InitAsync();
        var rawList = await _context.AsyncDbConnection.Table<DnsObj>().ToListAsync();
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
        return await _context.AsyncDbConnection.DeleteAsync(entity) > 0;
    }

    public async Task<DnsObj> FindByAsync(int id)
    {
        await _context.InitAsync();
        return await _context.AsyncDbConnection.GetAsync<DnsObj>(id);
    }

    public async Task<DnsObjViewModel> FindByAsync(string dns)
    {
        await _context.InitAsync();
        var entity = await _context.AsyncDbConnection.Table<DnsObj>().FirstOrDefaultAsync(x=>x.FirstDns == dns || x.SecondDns == dns);
        return new DnsObjViewModel(entity.Id, entity.Name, entity.FirstDns,entity.SecondDns);
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
        var result = await _context.AsyncDbConnection.Table<DnsObj>().FirstOrDefaultAsync(expression);
        
        if (result is null)
            return false;

        return true;
    }

    public async Task<DnsObjViewModel> FindByAsync(Expression<Func<DnsObj, bool>> expression)
    {
        await _context.InitAsync();
        var dnsObj = await _context.AsyncDbConnection.Table<DnsObj>().Where(expression).FirstOrDefaultAsync();
        return dnsObj.MapDnsObjViewModel();
    }

    public async Task<bool> IsDuplicateDnsAsync(string name, string firstDns, string secondDns)
    {
        await _context.InitAsync();
        var dns = await _context.AsyncDbConnection.Table<DnsObj>().
            FirstOrDefaultAsync(x => x.Name == name || (x.FirstDns == firstDns && x.SecondDns == secondDns) || (x.FirstDns == secondDns && x.SecondDns == firstDns));
        
        return dns is not null;
    }

    public bool Create(DnsObj entity)
    {
        _context.Init();
        return _context.SyncDbConnection.Insert(entity) > 0;
    }

    public bool Update(DnsObj entity)
    {
        _context.Init();
        return _context.SyncDbConnection.Update(entity) > 0;
    }

    public List<DnsObjViewModel> GetAll()
    {
        _context.Init();
        return _context.SyncDbConnection.Table<DnsObj>()
            .Select(x=> x.MapDnsObjViewModel() )
        .OrderByDescending(o=>o.Id)
        .ToList();
    }

    public bool Delete(int id)
    {
        _context.Init();
        return _context.SyncDbConnection.Delete<DnsObj>(id) > 0;
    }

    public DnsObj FindBy(int id)
    {
        _context.Init();
        return _context.SyncDbConnection.Find<DnsObj>(id);
    }

    public DnsObjViewModel FindBy(string dns)
    {
        return _context.SyncDbConnection.Table<DnsObj>()
            .Select(x=>x.MapDnsObjViewModel())
            .FirstOrDefault(x=>x.FirstDns == dns || x.SecondDns == dns);
    }

    public EditDnsObj GetDetail(int id)
    {
        _context.Init();
        var entity = FindBy(id);
        return new EditDnsObj(entity.FirstDns,entity.SecondDns, entity.Name, entity.Id);
    }

    public bool Exists(Expression<Func<DnsObj, bool>> expression)
    {
        _context.Init();
        return _context.SyncDbConnection.Table<DnsObj>().Any(expression.Compile());
    }

    public DnsObjViewModel FindBy(Expression<Func<DnsObj, bool>> expression)
    {
        _context.Init();
        return _context.SyncDbConnection.Table<DnsObj>()
            .Where(expression).Select(x => x.MapDnsObjViewModel()).FirstOrDefault();
    }

    public bool IsDuplicateDns(string name, string firstDns, string secondDns)
    {
        _context.Init();
        return _context.SyncDbConnection.Table<DnsObj>()
            .Any(x => x.Name == name || (x.FirstDns == firstDns && x.SecondDns == secondDns) || (x.FirstDns == secondDns && x.SecondDns == firstDns));
    }
}