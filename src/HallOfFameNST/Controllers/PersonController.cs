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
        private readonly ILogger<PersonController> _logger;

        private readonly IPersonService _personService;

        public PersonController(ILogger<PersonController> logger,
                                IPersonService personService)
        {
            _logger = logger;
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
            _logger.LogInformation("Starting GetPersons endpoint");
            try
            {
                var persons = await _personService.GetPersonsAsync();
                _logger.LogInformation("Successfully retrieved {Count} persons", persons.Count());
                return Ok(persons);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get a list of persons. Error: {Message}", ex.Message);
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
            _logger.LogInformation("Starting GetPerson endpoint for id={Id}", id);
            try
            {
                var person = await _personService.GetPersonByIdAsync(id);
                _logger.LogInformation("Successfully retrieved person with id={Id}", id);
                return Ok(person);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Person with id={id} not found", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get a person with id={id}. Error: {Message}", id, ex.Message);
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
            _logger.LogInformation("Starting CreatePerson endpoint");
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state while creating a person " +
                                       "with id={id}. Errors: {Errors}",
                                        personDto.Id, 
                                        ModelState.Values.SelectMany(v => v.Errors)
                                                         .Select(e => e.ErrorMessage));
                    return BadRequest(ModelState);
                }

                var createdPerson = await _personService.CreatePersonAsync(personDto);
                _logger.LogInformation("Successful created a person with id={id}", createdPerson.Id);
                return CreatedAtAction(nameof(GetPerson), new { id = createdPerson.Id }, createdPerson);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to create a person. Error: {Message}", ex.Message);
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
            _logger.LogInformation("Starting UpdatePerson endpoint for id={Id}", id);
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state while updating person " +
                                       "with id={Id}. Errors: {Errors}",
                                       id, 
                                       ModelState.Values.SelectMany(v => v.Errors)
                                                        .Select(e => e.ErrorMessage));
                    return BadRequest(ModelState);
                }

                await _personService.UpdatePersonAsync(id, personDto);
                _logger.LogInformation("Successfully updated person with id={Id}", id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Person with id={id} not found", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to update a person with id={Id}. Error: {Message}", 
                                 id, ex.Message);
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
            _logger.LogInformation("Starting DeletePerson endpoint for id={Id}", id);
            try
            {
                await _personService.DeletePersonAsync(id);
                _logger.LogInformation("Successfully deleted person with id={Id}", id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Person with id={id} not found", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to delete a person with id={Id}. Error: {Message}",
                                 id, ex.Message);
                throw;
            }
        }
    }
}