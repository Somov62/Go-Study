using DataBaseCore.Entities;
using SQLite;
using System;

namespace DataBaseCore
{
    public partial class DbContext : IDisposable
    {
        private readonly SQLiteAsyncConnection _database;
        public DbContext(string databasePath)
        {
            _database = new SQLiteAsyncConnection(databasePath);
            _database.CreateTableAsync<UserDataModel>();
        }

        public SQLiteAsyncConnection Database { get => _database; }

        public void Dispose()
        {
            _context = null;
            _database.CloseAsync();
            //_database.Dis();
            GC.SuppressFinalize(this);
        }
    }
}
