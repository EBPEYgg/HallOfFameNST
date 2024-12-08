using HallOfFameNST.Model;
using Microsoft.EntityFrameworkCore;

namespace HallOfFameNST.Data
{
    public class HallOfFameNSTContext : DbContext
    {
        public DbSet<Person> Person { get; set; }

        public DbSet<Skill> Skills { get; set; }

        public HallOfFameNSTContext(DbContextOptions<HallOfFameNSTContext> options) : base(options)
        {

        }
    }
}