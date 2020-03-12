using Microsoft.EntityFrameworkCore;
 
namespace server.Model
{
    public class bdContext : DbContext
    {
        public DbSet<Message> Messages { get; set; }
        public bdContext(DbContextOptions<bdContext> options)
            : base(options)
        {
            Database.EnsureCreated();   // создаем базу данных при первом обращении
        }
    }
}