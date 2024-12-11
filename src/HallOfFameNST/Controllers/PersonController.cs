using Microsoft.AspNetCore.Mvc;
using HallOfFameNST.Services.Interfaces;
using HallOfFameNST.DTO;
using HallOfFameNST.Model;

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
        /// <returns>Если успешно, то массив объектов типа <see cref="Person"/>
        /// и <see cref="StatusCodes.Status201Created"/>; <br/>
        /// Иначе <see cref="Exception"/>.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonDto>>> GetPersons()
        {
            try
            {
                var persons = await _personService.GetPersonsAsync();
                return Ok(persons);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Возвращает сотрудника с указанным id.
        /// </summary>
        /// <param name="id">Уникальный идентификатор сотрудника.</param>
        /// <returns>Если успешно, то объект типа <see cref="Person"/>
        /// и <see cref="StatusCodes.Status200OK"/>;<br/>
        /// Если сотрудник не найден, то <see cref="StatusCodes.Status404NotFound"/>;<br/>
        /// Иначе <see cref="Exception"/>.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PersonDto>> GetPerson(long id)
        {
            try
            {
                var person = await _personService.GetPersonByIdAsync(id);
                return Ok(person);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Создает нового сотрудника в системе с указанными навыками.
        /// </summary>
        /// <param name="person">Сотрудник.</param>
        /// <returns>Если успешно, то <see cref="StatusCodes.Status201Created"/>; <br/> 
        /// Если не пройдена валидация модели, то <see cref="StatusCodes.Status400BadRequest"/>; <br/>
        /// Иначе <see cref="Exception"/>.</returns>
        [HttpPost]
        public async Task<ActionResult<PersonDto>> CreatePerson(PersonDto personDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdPerson = await _personService.CreatePersonAsync(personDto);
                return CreatedAtAction(nameof(GetPerson), new { id = createdPerson.Id }, createdPerson);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Обновляет данные сотрудника и его навыки согласно значениям.
        /// </summary>
        /// <param name="id">Уникальный идентификатор сотрудника.</param>
        /// <param name="person">Сотрудник.</param>
        /// <returns>Если успешно, то <see cref="StatusCodes.Status204NoContent"/>; <br/>
        /// Если сотрудник не найден, то <see cref="StatusCodes.Status404NotFound"/>; <br/>
        /// Если не пройдена валидация модели, то <see cref="StatusCodes.Status400BadRequest"/>; <br/>
        /// Иначе <see cref="Exception"/>.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePerson(long id, PersonDto personDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _personService.UpdatePersonAsync(id, personDto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Удаляет сотрудника с указанным id из системы.
        /// </summary>
        /// <param name="id">Уникальный идентификатор сотрудника.</param>
        /// <returns>Если успешно, то <see cref="StatusCodes.Status204NoContent"/>; <br/>
        /// Если сотрудник не найден, то <see cref="StatusCodes.Status404NotFound"/>; <br/>
        /// Иначе <see cref="Exception"/>.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePerson(long id)
        {
            try
            {
                await _personService.DeletePersonAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}