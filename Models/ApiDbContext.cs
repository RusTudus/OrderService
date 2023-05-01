using Microsoft.EntityFrameworkCore;

namespace OrderService.Models
{
    public class ApiDbContext:DbContext
    {
        public ApiDbContext(DbContextOptions option) : base(option) 
        {
            Database.EnsureCreated();
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<Good> Goods { get; set; }
    }
}
