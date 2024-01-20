using DC.Domain.DnsObj;
using SQLite;

namespace DC.Infrastructure.SQlitePCL;

public class DnsContext 
{
    private readonly string _dbPath;
    public SQLiteAsyncConnection Database;
    
    private const SQLite.SQLiteOpenFlags Flags =
        // open the database in read/write mode
        SQLite.SQLiteOpenFlags.ReadWrite |
        // create the database if it doesn't exist
        SQLite.SQLiteOpenFlags.Create |
        // enable multi-threaded database access
        SQLite.SQLiteOpenFlags.SharedCache;

    public DnsContext(string dbPath)
    {
        _dbPath = dbPath;
        InitAsync();
    }

    public async Task InitAsync()
    {
        if (Database is not null)
        {
            return;
        }
        Database = new SQLiteAsyncConnection(_dbPath, Flags);
        await Database.CreateTableAsync<DnsObj>();
    }
}