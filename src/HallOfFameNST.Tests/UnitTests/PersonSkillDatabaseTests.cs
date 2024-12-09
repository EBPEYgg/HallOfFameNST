using HallOfFameNST.Data;
using HallOfFameNST.Model;
using Microsoft.EntityFrameworkCore;

namespace HallOfFameNST.Tests.UnitTests
{
    public class PersonSkillDatabaseTests
    {
        [Fact]
        public void DatabaseContext_ShouldCreateTablesForModels()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<HallOfFameNSTContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;

            // Act
            using var context = new HallOfFameNSTContext(options);

            // Assert
            context.Database.EnsureCreated();
            context.Person.Should().NotBeNull();
            context.Skills.Should().NotBeNull();
        }

        [Fact]
        public async Task AddPerson_ShouldSaveToDatabase()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<HallOfFameNSTContext>()
                .UseInMemoryDatabase("AddPersonTest")
                .Options;

            using var context = new HallOfFameNSTContext(options);
            var person = new Person
            {
                Name = "John Doe",
                DisplayName = "JD",
                Skills = new List<Skill>
                {
                    new Skill { Name = "C#", Level = 8 }
                }
            };

            // Act
            await context.Person.AddAsync(person);
            await context.SaveChangesAsync();

            // Assert
            var savedPerson = await context.Person
                .Include(p => p.Skills)
                .FirstOrDefaultAsync(p => p.Name == "John Doe");
            savedPerson.Should().NotBeNull();
            savedPerson.Skills.Should().HaveCount(1);
            savedPerson.Skills.First().Name.Should().Be("C#");
        }

        [Fact]
        public async Task AddSkill_ShouldLinkToPerson()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<HallOfFameNSTContext>()
                .UseInMemoryDatabase("AddSkillTest")
                .Options;

            using var context = new HallOfFameNSTContext(options);
            var person = new Person { Name = "Jane Doe", DisplayName = "JD" };
            await context.Person.AddAsync(person);
            await context.SaveChangesAsync();

            var skill = new Skill { Name = "Python", Level = 7, PersonId = person.Id };

            // Act
            await context.Skills.AddAsync(skill);
            await context.SaveChangesAsync();

            // Assert
            var savedSkill = await context.Skills.FirstOrDefaultAsync(s => s.Name == "Python");
            savedSkill.Should().NotBeNull();
            savedSkill.PersonId.Should().Be(person.Id);
        }

        [Fact]
        public async Task DeletePerson_ShouldRemoveAssociatedSkills()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<HallOfFameNSTContext>()
                .UseInMemoryDatabase("DeletePersonTest")
                .Options;

            using var context = new HallOfFameNSTContext(options);
            var person = new Person
            {
                Name = "Jane Doe",
                DisplayName = "JD",
                Skills = new List<Skill>
                {
                    new Skill { Name = "C#", Level = 5 },
                    new Skill { Name = "SQL", Level = 4 }
                }
            };
            await context.Person.AddAsync(person);
            await context.SaveChangesAsync();

            // Act
            context.Person.Remove(person);
            await context.SaveChangesAsync();

            // Assert
            var savedPerson = await context.Person.FirstOrDefaultAsync(p => p.Id == person.Id);
            var savedSkills = await context.Skills.Where(s => s.PersonId == person.Id).ToListAsync();

            savedPerson.Should().BeNull();
            savedSkills.Should().BeEmpty();
        }
    }
}