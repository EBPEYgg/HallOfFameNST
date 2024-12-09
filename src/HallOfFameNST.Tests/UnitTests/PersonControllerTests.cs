using HallOfFameNST.Controllers;
using HallOfFameNST.DTO;
using HallOfFameNST.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace HallOfFameNST.Tests.UnitTests
{
    public class PersonControllerTests
    {
        private readonly Mock<ILogger<PersonController>> _loggerMock;

        private readonly Mock<IPersonService> _personServiceMock;

        private readonly PersonController _controller;

        public PersonControllerTests()
        {
            _loggerMock = new Mock<ILogger<PersonController>>();
            _personServiceMock = new Mock<IPersonService>();
            _controller = new PersonController(_loggerMock.Object, _personServiceMock.Object);
        }

        [Fact]
        public async Task GetPerson_ShouldReturnPerson_WhenPersonExists()
        {
            // Arrange
            var personDto = new PersonDto { Name = "John Doe", DisplayName = "John" };
            var test = _personServiceMock.Setup(service => service.GetPersonByIdAsync(personDto.Id)).ReturnsAsync(personDto);

            // Act
            var result = await _controller.GetPerson(personDto.Id);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();

            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().NotBeNull();

            var returnedPerson = okResult.Value as PersonDto;
            returnedPerson.Should().NotBeNull();
            returnedPerson.Name.Should().Be("John Doe");
            returnedPerson.DisplayName.Should().Be("John");
        }

        [Fact]
        public async Task GetPerson_ShouldReturnNotFound_WhenPersonDoesNotExist()
        {
            // Arrange
            long personId = 1;
            _personServiceMock.Setup(service => service.GetPersonByIdAsync(personId))
                                                       .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.GetPerson(personId);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetPersons_ShouldReturnListOfPersons_WhenPersonsExists()
        {
            // Arrange
            var personsDto = new List<PersonDto>
            {
                new PersonDto { Name = "Alice", DisplayName = "Ali" },
                new PersonDto { Name = "Bob", DisplayName = "B0b" }
            }.AsQueryable();

            _personServiceMock.Setup(service => service.GetPersonsAsync()).ReturnsAsync(personsDto);

            // Act
            var result = await _controller.GetPersons();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();

            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().NotBeNull();

            var persons = okResult.Value as IEnumerable<PersonDto>;
            persons.Should().NotBeNull();
            persons.Should().HaveCount(2);
            persons.Should().ContainSingle(p => p.Name == "Alice");
        }

        [Fact]
        public async Task GetPersons_ShouldReturnEmptyList_WhenNoPersonsExist()
        {
            // Arrange
            _personServiceMock.Setup(service => service.GetPersonsAsync())
                                                       .ReturnsAsync(new List<PersonDto>());

            // Act
            var result = await _controller.GetPersons();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();

            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().NotBeNull();

            var persons = okResult.Value as IEnumerable<PersonDto>;
            persons.Should().NotBeNull();
            persons.Should().BeEmpty();
        }

        [Fact]
        public async Task CreatePerson_ShouldReturnCreated_WhenValidDataProvided()
        {
            // Arrange
            var person = new PersonDto { Name = "John Doe", DisplayName = "John" };
            _personServiceMock.Setup(service => service.CreatePersonAsync(It.IsAny<PersonDto>()))
                                                       .ReturnsAsync(person);

            // Act
            var result = await _controller.CreatePerson(person);

            // Assert
            result.Result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Value.Should().NotBeNull();
            createdResult.Value.Should().BeEquivalentTo(person);
        }

        [Fact]
        public async Task CreatePerson_ShouldReturnBadRequest_WhenDataIsInvalid()
        {
            // Arrange
            var person = new PersonDto { Name = "", DisplayName = "John" };
            _controller.ModelState.AddModelError("Name", "Name is required.");

            // Act
            var result = await _controller.CreatePerson(person);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UpdatePerson_ShouldReturnNoContent_WhenPersonExists()
        {
            // Arrange
            var personDto = new PersonDto { Name = "John Doe", DisplayName = "Johnny" };
            var updatedPersonDto = new PersonDto { Name = "Jane Doe", DisplayName = "Jane" };
            _personServiceMock.Setup(service => service.UpdatePersonAsync(personDto.Id, updatedPersonDto))
                                                       .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdatePerson(personDto.Id, updatedPersonDto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdatePerson_ShouldReturnNotFound_WhenPersonDoesNotExist()
        {
            // Arrange
            var updatedPersonDto = new PersonDto { Name = "Jane Doe", DisplayName = "Jane" };
            _personServiceMock.Setup(service => service.UpdatePersonAsync(updatedPersonDto.Id, updatedPersonDto))
                                                       .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.UpdatePerson(updatedPersonDto.Id, updatedPersonDto);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeletePerson_ShouldReturnNoContent_WhenPersonExists()
        {
            // Arrange
            var personDto = new PersonDto { Name = "John Doe", DisplayName = "John" };
            _personServiceMock.Setup(service => service.DeletePersonAsync(personDto.Id))
                                                       .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeletePerson(personDto.Id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeletePerson_ShouldReturnNotFound_WhenPersonDoesNotExist()
        {
            // Arrange
            long personId = 1;
            _personServiceMock.Setup(service => service.DeletePersonAsync(personId))
                                                       .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.DeletePerson(personId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}