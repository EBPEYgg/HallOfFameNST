using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HallOfFameNST.Model.Classes;
using HallOfFameNST.Model.Data;

namespace HallOfFameNST.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
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

        // GET: /api/v1/persons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersons()
        {
            _logger.LogInformation("Starting GetPersons endpoint");
            try
            {
                var persons = await _context.Person.Include(p => p.Skills).ToListAsync();
                _logger.LogInformation("Successfully retrieved {Count} persons", persons.Count);
                return persons;
        }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get a list of persons. Error: {ex.Message}");
                throw;
            }
        }

        // GET: api/v1/persons/{id}
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

            if (ModelState.IsValid)
            {
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
                existingPerson.Skills = person.Skills;

                //_context.Update(person);
                await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully updated person with id={Id}", id);
            return NoContent();
        }

        // POST: api/v1/persons/{id}
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