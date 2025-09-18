using DC.Core.Domain.DnsObj;
using SQLite;

namespace DC.Core.Infrastructure.SQLItePCL;

public class DnsContext 
{
    private readonly string _dbPath;
    public SQLiteAsyncConnection? AsyncDbConnection;
    public SQLiteConnection? SyncDbConnection;
    
    private const SQLite.SQLiteOpenFlags Flags =
        // open the database in read/write mode
        SQLite.SQLiteOpenFlags.ReadWrite |
        // create the database if it doesn't exist
        SQLite.SQLiteOpenFlags.Create |
        // enable multi-threaded database access
        SQLite.SQLiteOpenFlags.SharedCache;

    public DnsContext(string dbPath,bool isAsync = false)
    {
        _dbPath = dbPath;
        if (isAsync)
        {
            Task.Run(async () => await InitAsync());
        }
        else
        {
            Init();
        }
    }

    public async Task InitAsync()
    {
        if (AsyncDbConnection is not null)
        {
            return;
        }
        AsyncDbConnection = new SQLiteAsyncConnection(_dbPath, Flags);
        await AsyncDbConnection.CreateTableAsync<DnsObj>();
    }
    
    public void Init()
    {
        if (SyncDbConnection is not null)
        {
            return;
        }
        SyncDbConnection = new SQLiteConnection(_dbPath, Flags);
        SyncDbConnection.CreateTable<DnsObj>();
    }
}