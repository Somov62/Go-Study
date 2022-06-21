using System;
using System.IO;

namespace DataBaseCore
{
    public partial class DbContext
    {
        private const string _databaseName = "GoStudyLocalDb.db";
        private static readonly string _connectionString = Path.Combine
            (Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _databaseName);

        private static DbContext _context;

        public static DbContext GetContext()
        {
            if (_context == null)
            {
                _context = new DbContext(_connectionString);
            }
            return _context;
        }
    }
}
