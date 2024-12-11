using HallOfFameNST.Model;
using HallOfFameNST.Repository.Interfaces;
using Moq;

namespace HallOfFameNST.Tests.UnitTests
{
    public class PersonSkillDatabaseTests
    {
        private readonly Mock<IPersonRepository> _personRepositoryMock;

        public PersonSkillDatabaseTests()
        {
            _personRepositoryMock = new Mock<IPersonRepository>();
        }

        [Fact]
        public async Task Repository_ShouldContainCorrectModels()
        {
            // Arrange
            var persons = new List<Person>
            {
                new Person { Name = "John Doe", DisplayName = "John" },
                new Person { Name = "Jane McQueen", DisplayName = "Jane" }
            };

            _personRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(persons);

            // Act
            var savedPersons = await _personRepositoryMock.Object.GetAllAsync();

            // Assert
            savedPersons.Should().HaveCount(2);
            savedPersons.Should().Contain(p => p.Name == "John Doe");
            savedPersons.Should().Contain(p => p.Name == "Jane McQueen");
        }

        [Fact]
        public async Task AddPerson_ShouldSaveToRepository()
        {
            // Arrange
            var person = new Person
            {
                Name = "John Doe",
                DisplayName = "John",
                Skills = new List<Skill>
                {
                    new Skill { Name = "C#", Level = 8 }
                }
            };

            _personRepositoryMock.Setup(repo => repo.AddAsync(person)).Returns(Task.CompletedTask);
            _personRepositoryMock.Setup(repo => repo.GetByIdAsync(person.Id)).ReturnsAsync(person);

            // Act
            await _personRepositoryMock.Object.AddAsync(person);
            var savedPerson = await _personRepositoryMock.Object.GetByIdAsync(person.Id);

            // Assert
            savedPerson.Skills.Should().HaveCount(1);
            savedPerson.Skills.Should().ContainSingle(skill => skill.Name == "C#");
        }

        [Fact]
        public async Task AddSkill_ShouldLinkToPerson()
        {
            // Arrange
            var person = new Person { Name = "Jane McQueen", DisplayName = "Jane" };
            var skill = new Skill { Name = "Python", Level = 7, PersonId = person.Id };
            _personRepositoryMock.Setup(repo => repo.GetByIdAsync(person.Id)).ReturnsAsync(person);
            person.Skills = new List<Skill> { skill };
            _personRepositoryMock.Setup(repo => repo.UpdateAsync(person)).Returns(Task.CompletedTask);

            // Act
            await _personRepositoryMock.Object.UpdateAsync(person);
            var savedPerson = await _personRepositoryMock.Object.GetByIdAsync(person.Id);

            // Assert
            savedPerson.Skills.Should().NotBeNull();
            savedPerson.Skills.Should().HaveCount(1);
            savedPerson.Skills.First().PersonId.Should().Be(person.Id);
            savedPerson.Skills.Should().Contain(s => s.Name == "Python" && s.Level == 7);
        }

        [Fact]
        public async Task UpdatePerson_ShouldUpdatePersonInRepository()
        {
            // Arrange
            var person = new Person
            {
                Name = "Jane McQueen",
                DisplayName = "Jane",
                Skills = new List<Skill>
                {
                    new Skill { Name = "C#", Level = 1 }
                }
            };

            _personRepositoryMock.Setup(repo => repo.GetByIdAsync(person.Id)).ReturnsAsync(person);
            _personRepositoryMock.Setup(repo => repo.UpdateAsync(person)).Returns(Task.CompletedTask);

            // Act
            var updatedPerson = await _personRepositoryMock.Object.GetByIdAsync(person.Id);
            updatedPerson.Name = "John Smith";
            updatedPerson.DisplayName = "John";
            updatedPerson.Skills.First().Level = 5;
            await _personRepositoryMock.Object.UpdateAsync(updatedPerson);
            var savedPerson = await _personRepositoryMock.Object.GetByIdAsync(person.Id);

            // Assert
            savedPerson.Name.Should().Be("John Smith");
            savedPerson.DisplayName.Should().Be("John");
            savedPerson.Skills.First().Level.Should().Be(5);
        }

        [Fact]
        public async Task DeletePerson_ShouldRemoveAssociatedSkills()
        {
            // Arrange
            var person = new Person
            {
                Name = "Jane Doe",
                DisplayName = "Jane",
                Skills = new List<Skill>
                {
                    new Skill { Name = "C#", Level = 5 },
                    new Skill { Name = "SQL", Level = 4 }
                }
            };

            _personRepositoryMock.Setup(repo => repo.GetByIdAsync(person.Id)).ReturnsAsync(person);
            _personRepositoryMock.Setup(repo => repo.DeleteAsync(person)).Returns(Task.CompletedTask);
            _personRepositoryMock.Setup(repo => repo.GetByIdAsync(person.Id)).ReturnsAsync((Person)null);

            // Act
            await _personRepositoryMock.Object.DeleteAsync(person);
            var savedPerson = await _personRepositoryMock.Object.GetByIdAsync(person.Id);

            // Assert
            savedPerson.Should().BeNull();
        }
    }
}