using HallOfFameNST.DTO;
using HallOfFameNST.Services.Interfaces;
using Moq;

namespace HallOfFameNST.Tests.UnitTests
{
    public class PersonServiceTests
    {
        private readonly Mock<IPersonService> _personServiceMock;

        public PersonServiceTests()
        {
            _personServiceMock = new Mock<IPersonService>();
        }

        [Fact]
        public async Task GetPerson_ShouldReturnPerson_WhenPersonExists()
        {
            // Arrange
            var personDto = new PersonDto { Name = "John Doe", DisplayName = "John" };
            _personServiceMock.Setup(service => service.GetPersonByIdAsync(personDto.Id)).ReturnsAsync(personDto);

            // Act
            var returnedPersonDto = await _personServiceMock.Object.GetPersonByIdAsync(personDto.Id);

            // Assert
            returnedPersonDto.Should().NotBeNull();
            returnedPersonDto.Name.Should().Be("John Doe");
            returnedPersonDto.DisplayName.Should().Be("John");
        }

        [Fact]
        public async Task GetPersonByIdAsync_ShouldThrowException_WhenPersonDoesNotExist()
        {
            // Arrange
            long personDtoId = 1;
            _personServiceMock.Setup(service => service.GetPersonByIdAsync(personDtoId))
                                                       .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = async () => await _personServiceMock.Object.GetPersonByIdAsync(personDtoId);

            // Assert
            await result.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task GetPersons_ShouldReturnListOfPersons_WhenPersonsExists()
        {
            // Arrange
            var personsDto = new List<PersonDto>
            {
                new PersonDto { Name = "Alice", DisplayName = "Ali" },
                new PersonDto { Name = "Bob", DisplayName = "B0b" }
            };

            _personServiceMock.Setup(service => service.GetPersonsAsync()).ReturnsAsync(personsDto);

            // Act
            var returnedPersonsDto = await _personServiceMock.Object.GetPersonsAsync();

            // Assert
            returnedPersonsDto.Should().NotBeNull();
            returnedPersonsDto.Should().HaveCount(2);
            returnedPersonsDto.Should().ContainSingle(p => p.Name == "Alice");
            returnedPersonsDto.Should().ContainSingle(p => p.Name == "Bob");
        }

        [Fact]
        public async Task GetPersons_ShouldReturnEmptyList_WhenNoPersonsExist()
        {
            // Arrange
            _personServiceMock.Setup(service => service.GetPersonsAsync())
                                                       .ReturnsAsync(new List<PersonDto>());

            // Act
            var personsDto = await _personServiceMock.Object.GetPersonsAsync();

            // Assert
            personsDto.Should().NotBeNull();
            personsDto.Should().BeEmpty();
        }

        [Fact]
        public async Task CreatePersonAsync_ShouldReturnCreatedPerson_WhenValidDataProvided()
        {
            // Arrange
            var personDto = new PersonDto { Name = "John Doe", DisplayName = "John" };
            _personServiceMock.Setup(service => service.CreatePersonAsync(personDto)).ReturnsAsync(personDto);

            // Act
            var result = await _personServiceMock.Object.CreatePersonAsync(personDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(personDto);
        }

        [Fact]
        public async Task UpdatePersonAsync_ShouldComplete_WhenPersonExists()
        {
            // Arrange
            var personDto = new PersonDto { Name = "John Doe", DisplayName = "Johnny" };
            var updatedPersonDto = new PersonDto { Name = "Jane Doe", DisplayName = "Jane" };
            _personServiceMock.Setup(service => service.UpdatePersonAsync(personDto.Id, updatedPersonDto))
                                                       .Returns(Task.CompletedTask);

            // Act
            var result = async () => await _personServiceMock.Object.UpdatePersonAsync(
                personDto.Id, updatedPersonDto);

            // Assert
            await result.Should().NotThrowAsync();
        }

        [Fact]
        public async Task UpdatePersonAsync_ShouldThrowException_WhenPersonDoesNotExist()
        {
            // Arrange
            var updatedPersonDto = new PersonDto { Name = "Jane Doe", DisplayName = "Jane" };
            _personServiceMock.Setup(service => service.UpdatePersonAsync(updatedPersonDto.Id, updatedPersonDto))
                                                       .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = async() => await _personServiceMock.Object.UpdatePersonAsync(
                updatedPersonDto.Id, updatedPersonDto);

            // Assert
            await result.Should().ThrowAsync<KeyNotFoundException>();
        }

        [Fact]
        public async Task DeletePerson_ShouldComplete_WhenPersonExists()
        {
            // Arrange
            var personDto = new PersonDto { Name = "John Doe", DisplayName = "John" };
            _personServiceMock.Setup(service => service.DeletePersonAsync(personDto.Id))
                                                       .Returns(Task.CompletedTask);

            // Act
            var result = async () => await _personServiceMock.Object.DeletePersonAsync(personDto.Id);

            // Assert
            await result.Should().NotThrowAsync();
        }

        [Fact]
        public async Task DeletePersonAsync_ShouldThrowException_WhenPersonDoesNotExist()
        {
            // Arrange
            long personId = 1;
            _personServiceMock.Setup(service => service.DeletePersonAsync(personId))
                                                       .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = async () => await _personServiceMock.Object.DeletePersonAsync(personId);

            // Assert
            await result.Should().ThrowAsync<KeyNotFoundException>();
        }
    }
}