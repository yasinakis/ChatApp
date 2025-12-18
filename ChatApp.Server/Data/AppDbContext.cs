using Microsoft.EntityFrameworkCore;
using ChatApp.Server.Models;
namespace ChatApp.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Message> Messages { get; set; } = default!;
    }
}
