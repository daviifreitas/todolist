using Microsoft.EntityFrameworkCore;

namespace Activity.API.Context
{
    public class TaskDbContext : DbContext
    {
        public TaskDbContext(DbContextOptions<TaskDbContext> options)
            : base(options) { }
        public DbSet<Entities.ActivityTicket> Tasks { get; set; }
    }
}
