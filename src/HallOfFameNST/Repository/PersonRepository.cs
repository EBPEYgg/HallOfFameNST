using HallOfFameNST.Data;
using HallOfFameNST.Model;
using HallOfFameNST.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HallOfFameNST.Repository
{
    public class PersonRepository : IPersonRepository
    {
        private readonly HallOfFameNSTContext _context;

        public PersonRepository(HallOfFameNSTContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Person?>> GetAllAsync()
        {
            return await _context.Person
                .Include(p => p.Skills)
                .ToArrayAsync();
        }

        public async Task<Person> GetByIdAsync(long id)
        {
            return await _context.Person
                .Include(p => p.Skills)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Person person)
        {
            await _context.Person.AddAsync(person);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Person person)
        {
            _context.Person.Update(person);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Person person)
        {
            _context.Person.Remove(person);
            await _context.SaveChangesAsync();
        }
    }
}