using Microsoft.EntityFrameworkCore;
using SharedLibrary.Models;

namespace StreamService.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Content> Contents { get; set; }
    }
}