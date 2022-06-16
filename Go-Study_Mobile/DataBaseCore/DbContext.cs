using DataBaseCore.Entities;
using SQLite;
using System;

namespace DataBaseCore
{
    public partial class DbContext : IDisposable
    {
        private readonly SQLiteConnection _database;
        public DbContext(string databasePath)
        {
            _database = new SQLiteConnection(databasePath);
            _database.CreateTable<UserDataModel>();
        }

        public void Dispose()
        {
            _context = null;
            _database.Close();
            _database.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
