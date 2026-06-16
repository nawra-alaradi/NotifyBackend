using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NotifyBackend.Models;

namespace NotifyBackend
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
           : base(options)
        {
        }
        public virtual DbSet<Users> Users { get; set; }

        public virtual DbSet<Notes> Notes { get; set; }

        public virtual DbSet<Media> Media { get; set; }

        public virtual DbSet<CategoriesGate> CategoriesGate { get; set; }

        public virtual DbSet<CategoriesDefinitions> CategoriesDefinitions { get; set; }

    }
}
