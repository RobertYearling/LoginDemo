using Microsoft.EntityFrameworkCore;
using LoginDemo.Models;

namespace LoginDemo.Context
{
    public class HomeContext : DbContext
    {
        public HomeContext(DbContextOptions options) : base(options){}

        public DbSet<User> Users { get; set; }
    }
}