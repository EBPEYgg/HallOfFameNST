using HallOfFameNST.DTO;
using HallOfFameNST.Services;
using HallOfFameNST.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HallOfFameNST.Controllers
{
    [ApiController]
    [Route("api/v1/persons")]
    public class PersonController : Controller
    {
        private readonly IPersonService _personService;

        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }

        /// <summary>
        /// Возвращает всех сотрудников.
        /// </summary>
        /// <returns>Массив объектов типа <see cref="PersonDto"/>
        /// и <see cref="StatusCodes.Status201Created"/>.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonDto>>> GetPersons()
        {
            var persons = await _personService.GetPersonsAsync();
            return Ok(persons);
        }

        /// <summary>
        /// Возвращает сотрудника с указанным id.
        /// </summary>
        /// <param name="id">Уникальный идентификатор сотрудника.</param>
        /// <returns><see cref="OkObjectResult"/> 
        /// с объектом типа <see cref="PersonDto"/>.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PersonDto>> GetPerson(long id)
        {
            var person = await _personService.GetPersonByIdAsync(id);
            return Ok(person);
        }

        /// <summary>
        /// Создает нового сотрудника в системе с указанными навыками.
        /// </summary>
        /// <param name="personDto">Сотрудник.</param>
        /// <returns><see cref="CreatedAtActionResult"/>
        /// с объектом типа <see cref="PersonDto"/>.</returns>
        [HttpPost]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<ActionResult<PersonDto>> CreatePerson(PersonDto personDto)
        {
            var createdPerson = await _personService.CreatePersonAsync(personDto);
            return CreatedAtAction(nameof(GetPerson), new { id = createdPerson.Id }, createdPerson);
        }

        /// <summary>
        /// Обновляет данные сотрудника и его навыки согласно значениям.
        /// </summary>
        /// <param name="id">Уникальный идентификатор сотрудника.</param>
        /// <param name="personDto">Сотрудник.</param>
        /// <returns><see cref="NoContentResult"/> с 
        /// <see cref="StatusCodes.Status204NoContent"/>.</returns>
        [HttpPut("{id}")]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<IActionResult> UpdatePerson(long id, PersonDto personDto)
        {
            await _personService.UpdatePersonAsync(id, personDto);
            return NoContent();
        }

        /// <summary>
        /// Удаляет сотрудника с указанным id из системы.
        /// </summary>
        /// <param name="id">Уникальный идентификатор сотрудника.</param>
        /// <returns><see cref="NoContentResult"/> с 
        /// <see cref="StatusCodes.Status204NoContent"/>.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(long id)
        {
            await _personService.DeletePersonAsync(id);
            return NoContent();
        }
    }
}