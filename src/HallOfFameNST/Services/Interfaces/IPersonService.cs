using HallOfFameNST.DTO;

namespace HallOfFameNST.Services.Interfaces
{
    public interface IPersonService
    {
        Task<IEnumerable<PersonDto>> GetPersonsAsync();

        Task<PersonDto?> GetPersonByIdAsync(long id);

        Task<PersonDto> CreatePersonAsync(PersonDto personDto);

        Task UpdatePersonAsync(long id, PersonDto personDto);

        Task DeletePersonAsync(long id);
    }
}