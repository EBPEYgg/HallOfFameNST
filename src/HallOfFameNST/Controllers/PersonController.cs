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
        private readonly HallOfFameNSTContext _context;

        public PersonController(HallOfFameNSTContext context)
        {
            _context = context;
        }

        // GET: /api/v1/persons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersons()
        {
            return await _context.Person.Include(p => p.Skills).ToListAsync();
        }

        // GET: api/v1/persons/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Person>> GetPerson(long id)
        {
            var person = await _context.Person
                .Include(p => p.Skills)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (person == null)
            {
                return NotFound();
            }

            return person;
        }

        // POST: api/v1/persons
        [HttpPost]
        public async Task<ActionResult<Person>> CreatePerson(Person person)
        {
            if (ModelState.IsValid)
            {
                _context.Person.Add(person);
                await _context.SaveChangesAsync();
            }
            return CreatedAtAction(nameof(GetPerson), new { id = person.Id }, person);
        }

        // POST: api/v1/persons/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePerson(long id, Person person)
        {
            if (id != person.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingPerson = await _context.Person
                    .Include(p => p.Skills)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (existingPerson == null)
                {
                    return NotFound();
                }

                existingPerson.Name = person.Name;
                existingPerson.DisplayName = person.DisplayName;
                existingPerson.Skills = person.Skills;

                //_context.Update(person);
                await _context.SaveChangesAsync();
            }
            return NoContent();
        }

        // POST: api/v1/persons/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(long id)
        {
            var person = await _context.Person.FindAsync(id);

            if (person == null)
            {
                return NotFound();
            }

            _context.Person.Remove(person);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}