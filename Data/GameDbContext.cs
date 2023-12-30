using Game.Api.Model.Entity;
using Microsoft.EntityFrameworkCore;

namespace Game.Api.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
        {

        }

        public DbSet<Mode> Modes { get; set; }
    }
}
