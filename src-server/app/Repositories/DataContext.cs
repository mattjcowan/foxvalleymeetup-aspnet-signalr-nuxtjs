using app.Repositories;
using Microsoft.EntityFrameworkCore;

namespace app.Repositories
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } 
        public DbSet<Bookmark> Bookmarks { get; set; }
    }
}