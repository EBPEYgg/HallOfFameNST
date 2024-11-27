using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HallOfFameNST.Model.Classes;
using HallOfFameNST.Model.Data;

namespace HallOfFameNST.Controllers
{
    [ApiController]
    [Route("api/v1/persons")]
    public class PersonController : Controller
    {
        private readonly ILogger<PersonController> _logger;

        private readonly HallOfFameNSTContext _context;

        public PersonController(HallOfFameNSTContext context, 
                                ILogger<PersonController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/v1/persons
        /// <summary>
        /// Возвращает всех сотрудников.
        /// </summary>
        /// <returns>Массив объектов типа <see cref="Person"/>.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersons()
        {
            _logger.LogInformation("Starting GetPersons endpoint");
            try
            {
                var persons = await _context.Person.Include(p => p.Skills).ToArrayAsync();
                _logger.LogInformation("Successfully retrieved {Count} persons", persons.Count());
                return persons;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get a list of persons. Error: {ex.Message}");
                throw;
            }
        }

        // GET: api/v1/persons/{id}
        /// <summary>
        /// Возвращает сотрудника с указанным id.
        /// </summary>
        /// <param name="id">Уникальный идентификатор сотрудника.</param>
        /// <returns>Объект типа <see cref="Person"/>.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPerson(long id)
        {
            _logger.LogInformation("Starting GetPerson endpoint for id={Id}", id);

            var person = await _context.Person
                .Include(p => p.Skills)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (person == null)
            {
                _logger.LogWarning("Person with id={id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Successfully retrieved person with id={Id}", id);
            return person;
        }

        // POST: api/v1/persons
        /// <summary>
        /// Создает нового сотрудника в системе с указанными навыками.
        /// </summary>
        /// <param name="person">Сотрудник.</param>
        /// <returns>Если успешно, то <see cref="StatusCodes.Status201Created"/><br/> 
        /// Иначе <see cref="StatusCodes.Status400BadRequest"/>.</returns>
        [HttpPost]
        public async Task<ActionResult<Person>> CreatePerson(Person person)
        {
            _logger.LogInformation("Starting CreatePerson endpoint");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state while creating a person with id={id}. Errors: {Errors}", 
                                    person.Id, ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ModelState);
            }

            _context.Person.Add(person);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successful created a person with id={id}", person.Id);
            return CreatedAtAction(nameof(GetPerson), new { id = person.Id }, person);
        }

        // POST: api/v1/persons/{id}
        /// <summary>
        /// Обновляет данные сотрудника и его навыки согласно значениям.
        /// </summary>
        /// <param name="id">Уникальный идентификатор сотрудника.</param>
        /// <param name="person">Сотрудник.</param>
        /// <returns>Если успешно, то <see cref="StatusCodes.Status204NoContent"/>; <br/>
        /// Если сотрудник не найден, то <see cref="StatusCodes.Status404NotFound"/>; <br/>
        /// Иначе <see cref="StatusCodes.Status400BadRequest"/>.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePerson(long id, Person person)
        {
            _logger.LogInformation("Starting UpdatePerson endpoint for id={Id}", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state while updating person with id={Id}. Errors: {Errors}", 
                                    id, ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ModelState);
            }

            var existingPerson = await _context.Person
                .Include(p => p.Skills)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existingPerson == null)
            {
                _logger.LogWarning("Person with id={id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Updating details for person with id={Id}", id);
            existingPerson.Name = person.Name;
            existingPerson.DisplayName = person.DisplayName;
            var existingSkills = existingPerson.Skills.ToList();

            foreach (var newSkill in person.Skills)
            {
                var matchingSkill = existingSkills.FirstOrDefault(s => s.Name == newSkill.Name);
                // Если навык совпадает, обновляем уровень
                if (matchingSkill != null)
                {
                    _logger.LogInformation("Updating skill '{Skill}' for person with id={Id}", 
                                            newSkill.Name, id);
                    matchingSkill.Level = newSkill.Level;
                }
                // Если навык новый, добавляем его
                else
                {
                    _logger.LogInformation("Adding new skill '{Skill}' for person with id={Id}", 
                                            newSkill.Name, id);
                    existingPerson.Skills.Add(newSkill);
                }
            }

            // Удаление навыков, которых больше нет в новом списке
            foreach (var existingSkill in existingSkills)
            {
                if (!person.Skills.Any(s => s.Name == existingSkill.Name))
                {
                    _logger.LogInformation("Deleting skill '{Skill}' for person with id={Id}", 
                                            existingSkill.Name, id);
                    _context.Skills.Remove(existingSkill);
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully updated person with id={Id}", id);
            return NoContent();
        }

        // POST: api/v1/persons/{id}
        /// <summary>
        /// Удаляет сотрудника с указанным id из системы.
        /// </summary>
        /// <param name="id">Уникальный идентификатор сотрудника.</param>
        /// <returns>Если успешно, то <see cref="StatusCodes.Status204NoContent"/>; <br/>
        /// Если сотрудник не найден, то <see cref="StatusCodes.Status404NotFound"/>.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(long id)
        {
            _logger.LogInformation("Starting DeletePerson endpoint for id={Id}", id);
            var person = await _context.Person.FirstOrDefaultAsync(p => p.Id == id);

            if (person == null)
            {
                _logger.LogWarning("Person with id={Id} not found for deletion", id);
                return NotFound();
            }

            _context.Person.Remove(person);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Successfully deleted person with id={Id}", id);
            return NoContent();
        }
    }
}