using HallOfFameNST.DTO;
using HallOfFameNST.Model;
using HallOfFameNST.Repository.Interfaces;
using HallOfFameNST.Services.Interfaces;

namespace HallOfFameNST.Services
{
    public class PersonService : IPersonService
    {
        private readonly ILogger<PersonService> _logger;

        private readonly IPersonRepository _repository;

        public PersonService(IPersonRepository repository, 
                             ILogger<PersonService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<PersonDto>> GetPersonsAsync()
        {
            _logger.LogInformation("Retrieving all persons from the database.");
            var persons = await _repository.GetAllAsync();
            _logger.LogInformation("Successfully retrieved {Count} persons.", persons.Count());

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
            _logger.LogInformation("Retrieving person with id={id}.", id);
            var person = await _repository.GetByIdAsync(id);

            if (person == null)
            {
                _logger.LogWarning("Person with id={id} not found.", id);
                throw new KeyNotFoundException($"Person with id={id} not found.");
            }
            _logger.LogInformation("Successfully retrieved person with id={id}.", id);

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
            _logger.LogInformation("Creating a new person with id={id}.", personDto.Id);
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
            _logger.LogInformation("Successfully created a new person with id={id}.", person.Id);

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

        public async Task UpdatePersonAsync(long id, PersonDto personDto)
        {
            _logger.LogInformation("Updating person with id={id}.", id);
            var person = await _repository.GetByIdAsync(id);

            if (person == null)
            {
                _logger.LogWarning("Person with id={id} not found.", id);
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
            _logger.LogInformation("Successfully updated person with id={id}", id);
        }

        public async Task DeletePersonAsync(long id)
        {
            _logger.LogInformation("Deleting person with id={id}.", id);
            var person = await _repository.GetByIdAsync(id);

            if (person == null)
            {
                _logger.LogWarning("Person with id={id} not found.", id);
                throw new KeyNotFoundException($"Person with id={id} not found.");
            }

            await _repository.DeleteAsync(person);
            _logger.LogInformation("Successfully deleted person with id={id}", id);
        }
    }
}