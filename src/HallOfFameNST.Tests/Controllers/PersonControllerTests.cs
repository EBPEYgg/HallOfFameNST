using HallOfFameNST.Model.Classes;
using FluentAssertions;
using HallOfFameNST.Controllers;
using HallOfFameNST.Model.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Microsoft.AspNetCore.Mvc;

namespace HallOfFameNST.Tests.Controllers
{
    public class PersonControllerTests
    {
        private readonly Mock<ILogger<PersonController>> _loggerMock;

        private readonly HallOfFameNSTContext _context;

        private readonly PersonController _controller;

        public PersonControllerTests()
        {
            _context = CreateInMemoryContext();
            _loggerMock = new Mock<ILogger<PersonController>>();
            _controller = new PersonController(_context, _loggerMock.Object);
        }

        private HallOfFameNSTContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<HallOfFameNSTContext>()
                .UseInMemoryDatabase(databaseName: "HallOfFameTestDb")
                .Options;

            var context = new HallOfFameNSTContext(options);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            return context;
        }

        [Fact]
        public async Task GetPerson_ShouldReturnPerson_WhenPersonExists()
        {
            // Arrange
            _context.Person.Add(new Person { Name = "John Doe", DisplayName = "John" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetPerson(1);

            // Assert
            result.Value.Should().NotBeNull();
            result.Value.Name.Should().Be("John Doe");
        }

        [Fact]
        public async Task GetPersons_ShouldReturnListOfPersons_WhenPersonsExists()
        {
            // Arrange
            _context.Person.Add(new Person { Name = "John Doe", DisplayName = "John" });
            _context.Person.Add(new Person { Name = "Jane Doe", DisplayName = "Jane" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetPersons();

            // Assert
            result.Value.Should().NotBeNull();
            result.Value.Should().HaveCount(2);
            result.Value.Should().Contain(p => p.Name == "John Doe" && p.DisplayName == "John");
            result.Value.Should().Contain(p => p.Name == "Jane Doe" && p.DisplayName == "Jane");
        }

        [Fact]
        public async Task GetPerson_ShouldReturnNotFound_WhenPersonDoesNotExist()
        {
            // Act
            var result = await _controller.GetPerson(1);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetAllPersons_ShouldReturnEmptyList_WhenNoPersonsExist()
        {
            // Act
            var result = await _controller.GetPersons();

            // Assert
            result.Value.Should().NotBeNull();
            result.Value.Should().BeEmpty();
        }

        [Fact]
        public async Task CreatePerson_ShouldReturnCreated_WhenValidDataProvided()
        {
            // Arrange
            var person = new Person { Name = "John Doe", DisplayName = "John" };

            // Act
            var result = await _controller.CreatePerson(person);

            // Assert
            result.Result.Should().BeOfType<CreatedAtActionResult>();
            _context.Person.Should().HaveCount(1);
            _context.Person.First().Name.Should().Be("John Doe");
        }

        [Fact]
        public async Task CreatePerson_ShouldReturnBadRequest_WhenDataIsInvalid()
        {
            // Arrange
            var person = new Person { Name = "", DisplayName = "John" };
            _controller.ModelState.AddModelError("Name", "Name is required.");

            // Act
            var result = await _controller.CreatePerson(person);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            _context.Person.Should().BeEmpty();
        }

        [Fact]
        public async Task UpdatePerson_ShouldReturnNoContent_WhenPersonExists()
        {
            // Arrange=
            var person = new Person { Name = "John Doe", DisplayName = "Johnny" };
            _context.Person.Add(person);
            await _context.SaveChangesAsync();
            var updatedPerson = new Person { Name = "Jane Doe", DisplayName = "Jane" };

            // Act
            var result = await _controller.UpdatePerson(1, updatedPerson);
            var updated = await _context.Person.FirstOrDefaultAsync(p => p.Id == 1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            updated.Name.Should().Be("Jane Doe");
        }

        [Fact]
        public async Task UpdatePerson_ShouldReturnNotFound_WhenPersonDoesNotExist()
        {
            // Arrange
            var updatedPerson = new Person { Name = "Jane Doe", DisplayName = "Jane" };

            // Act
            var result = await _controller.UpdatePerson(1, updatedPerson);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeletePerson_ShouldReturnNoContent_WhenPersonExists()
        {
            // Arrange
            var person = new Person { Name = "John Doe", DisplayName = "John" };
            _context.Person.Add(person);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DeletePerson(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _context.Person.Should().BeEmpty();
        }

        [Fact]
        public async Task DeletePerson_ShouldReturnNotFound_WhenPersonDoesNotExist()
        {
            // Act
            var result = await _controller.DeletePerson(1);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}