using Microsoft.EntityFrameworkCore;

namespace idsserver
{
    public class MysqlApplicationDbContext : DbContext
    {
        public DbSet<UserAuth> UserAuth { get; set; }
        public MysqlApplicationDbContext(DbContextOptions<MysqlApplicationDbContext> options) : base(options)
        {
        }
    }
}