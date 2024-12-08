using HallOfFameNST.Model;

namespace HallOfFameNST.Repository.Interfaces
{
    public interface IPersonRepository
    {
        Task<IEnumerable<Person>> GetAllAsync();

        Task<Person?> GetByIdAsync(long id);

        Task AddAsync(Person person);

        Task UpdateAsync(Person person);

        Task DeleteAsync(Person person);
    }
}