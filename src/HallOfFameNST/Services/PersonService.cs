using HallOfFameNST.DTO;
using HallOfFameNST.Model;
using HallOfFameNST.Repository.Interfaces;
using HallOfFameNST.Services.Interfaces;

namespace HallOfFameNST.Services
{
    public class PersonService : IPersonService
    {
        private readonly IPersonRepository _repository;

        public PersonService(IPersonRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PersonDto>> GetPersonsAsync()
        {
            var persons = await _repository.GetAllAsync();

            return persons.Select(person => new PersonDto
            {
                Id = person.Id,
                Name = person.Name,
                DisplayName = person.DisplayName,
                Skills = person.Skills.Select(skill => new SkillDto
                {
                    Name = skill.Name,
                    Level = skill.Level
                }).ToArray()
            });
        }

        public async Task<PersonDto> GetPersonByIdAsync(long id)
        {
            var person = await _repository.GetByIdAsync(id);

            if (person == null)
            {
                throw new KeyNotFoundException($"Person with id={id} not found.");
            }

            return new PersonDto
            {
                Id = person.Id,
                Name = person.Name,
                DisplayName = person.DisplayName,
                Skills = person.Skills.Select(skill => new SkillDto
                {
                    Name = skill.Name,
                    Level = skill.Level
                }).ToArray()
            };
        }

        public async Task<PersonDto> CreatePersonAsync(PersonDto personDto)
        {
            var person = new Person
            {
                Name = personDto.Name,
                DisplayName = personDto.DisplayName,
                Skills = personDto.Skills.Select(skillDto => new Skill
                {
                    Name = skillDto.Name,
                    Level = skillDto.Level
                }).ToArray()
            };

            await _repository.AddAsync(person);
            return personDto;
        }

        public async Task UpdatePersonAsync(long id, PersonDto personDto)
        {
            var person = await _repository.GetByIdAsync(id);

            if (person == null)
            {
                throw new KeyNotFoundException($"Person with id={id} not found.");
            }

            person.Name = personDto.Name;
            person.DisplayName = personDto.DisplayName;

            foreach (var skillDto in personDto.Skills)
            {
                var existingSkill = person.Skills.FirstOrDefault(s => s.Name == skillDto.Name);
                if (existingSkill != null)
                {
                    existingSkill.Level = skillDto.Level;
                }
                else
                {
                    person.Skills.Add(new Skill
                    {
                        Name = skillDto.Name,
                        Level = skillDto.Level
                    });
                }
            }

            var skillsToRemove = person.Skills
                .Where(s => personDto.Skills.All(dto => dto.Name != s.Name))
                .ToList();

            foreach (var skill in skillsToRemove)
            {
                person.Skills.Remove(skill);
            }

            await _repository.UpdateAsync(person);
        }

        public async Task DeletePersonAsync(long id)
        {
            var person = await _repository.GetByIdAsync(id);

            if (person == null)
            {
                throw new KeyNotFoundException($"Person with id={id} not found.");
            }

            await _repository.DeleteAsync(person);
        }
    }
}