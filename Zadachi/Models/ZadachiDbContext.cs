using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Zadachi.Models
{
    public class ZadachiDbContext:IdentityDbContext
    {
        public ZadachiDbContext(DbContextOptions<ZadachiDbContext> options) : base(options)
        {
            //Database.EnsureCreated();   // создаем базу данных при первом обращении
        }

        public DbSet<Activity> Activities { get; set; }
    }
}
