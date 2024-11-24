using HallOfFameNST.Model.Classes;
using Microsoft.EntityFrameworkCore;

namespace HallOfFameNST.Model.Data
{
    public class HallOfFameNSTContext : DbContext
    {
        public DbSet<Person> Person {  get; set; }

        public DbSet<Skill> Skills { get; set; }

        public HallOfFameNSTContext(DbContextOptions<HallOfFameNSTContext> options) : base(options)
        {

        }
    }
}