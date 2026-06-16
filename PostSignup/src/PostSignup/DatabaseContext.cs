using Microsoft.EntityFrameworkCore;
using PostSignup.Models;

namespace PostSignup
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
           : base(options)
        {
        }
        public virtual DbSet<Users> Users { get; set; }

      

    }
}
