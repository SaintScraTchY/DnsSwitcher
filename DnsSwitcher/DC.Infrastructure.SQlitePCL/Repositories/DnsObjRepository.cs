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
    }

    public async Task<bool> CreateAsync(DnsObj entity)
    {
        return await _context.Database.InsertAsync(entity) > 0;
    }

    public async Task<bool> UpdateAsync(DnsObj entity)
    {
        return await _context.Database.UpdateAsync(entity) > 0;
    }

    public Task<List<DnsObjViewModel>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<DnsObj> FindByAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<DnsObjViewModel> FindByAsync(string dns)
    {
        throw new NotImplementedException();
    }

    public Task<EditDnsObj> GetDetailAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Exists(Expression<Func<DnsObj, bool>> expression)
    {
        throw new NotImplementedException();
    }
}