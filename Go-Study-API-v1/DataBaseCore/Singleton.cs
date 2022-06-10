using System.Data.Entity;
using System.Linq;

namespace DataBaseCore
{
    public partial class DbEntities : DbContext
    {
        private static DbEntities _context;

        public static DbEntities GetContext()
        {
            if (_context == null) 
                _context = new DbEntities();
            return _context;
        }
    }
}
