using System.Net;
using System.Net.Http.Json;
using HallOfFameNST.Model.Classes;

namespace HallOfFameNST.Tests.IntegrationTests
{
    public class IntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _webAppFactory;

        private readonly HttpClient _client;

        public IntegrationTests()
        {
            _webAppFactory = new CustomWebApplicationFactory<Program>();
            _client = _webAppFactory.CreateClient();
        }

        [Fact]
        public async Task GetPersons_ShouldReturnListOfPersons_WhenPersonsExists()
        {
            // Arrange
            var person1 = new Person { Name = "Alice Doe", DisplayName = "Alice" };
            var person2 = new Person { Name = "Bob Smith", DisplayName = "Bob" };

            await _client.PostAsJsonAsync("api/v1/persons", person1);
            await _client.PostAsJsonAsync("api/v1/persons", person2);

            // Act
            var response = await _client.GetAsync("api/v1/persons");
            var persons = await response.Content.ReadFromJsonAsync<List<Person>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            persons.Should().NotBeNull();
            persons.Should().HaveCount(2);

            persons[0].Name.Should().Be("Alice Doe");
            persons[0].DisplayName.Should().Be("Alice");

            persons[1].Name.Should().Be("Bob Smith");
            persons[1].DisplayName.Should().Be("Bob");
        }

        [Theory]
        [InlineData(0, HttpStatusCode.NotFound)]
        [InlineData(1, HttpStatusCode.NotFound)]
        [InlineData(-1, HttpStatusCode.NotFound)]
        [InlineData(2, HttpStatusCode.NotFound)]
        [InlineData(999, HttpStatusCode.NotFound)]
        public async Task GetPerson_ShouldReturnNotFound_WhenPersonDoesNotExist
            (long personId, HttpStatusCode expected)
        {
            // Act
            var response = await _client.GetAsync($"api/v1/persons/{personId}");

            // Assert
            response.StatusCode.Should().Be(expected);
        }

        [Fact]
        public async Task GetPersonById_ShouldReturnPerson_WhenPersonExists()
        {
            // Arrange
            var newPerson = new Person { Name = "Jane Doe", DisplayName = "Jane" };
            var createResponse = await _client.PostAsJsonAsync("api/v1/persons", newPerson);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdPerson = await createResponse.Content.ReadFromJsonAsync<Person>();
            long personId = createdPerson.Id;

            // Act
            var response = await _client.GetAsync($"api/v1/persons/{personId}");
            var person = await response.Content.ReadFromJsonAsync<Person>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            person.Should().NotBeNull();
            person.Id.Should().Be(personId);
            person.Name.Should().Be("Jane Doe");
            person.DisplayName.Should().Be("Jane");
        }

        [Fact]
        public async Task GetAllPersons_ShouldReturnEmptyList_WhenNoPersonsExist()
        {
            // Act
            var response = await _client.GetAsync("api/v1/persons");
            var result = await response.Content.ReadAsStringAsync();

            // Assert
            result.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task CreatePerson_ShouldReturnCreated_WhenValidDataProvided()
        {
            // Arrange
            var newPerson = new Person { Name = "John Doe", DisplayName = "John" };

            // Act
            var createResponse = await _client.PostAsJsonAsync("api/v1/persons", newPerson);
            var createdPerson = await createResponse.Content.ReadFromJsonAsync<object>();

            var response = await _client.GetAsync("api/v1/persons");
            var persons = await response.Content.ReadFromJsonAsync<List<Person>>();

            // Assert
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            createdPerson.Should().NotBeNull();
            persons[0].Name.Should().Be("John Doe");
            persons[0].DisplayName.Should().Be("John");
        }

        [Fact]
        public async Task CreatePerson_ShouldReturnBadRequest_WhenDataIsInvalid()
        {
            // Arrange
            var invalidPerson = new Person { Name = "", DisplayName = "" };

            // Act
            var response = await _client.PostAsJsonAsync("api/v1/persons", invalidPerson);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreatePerson_ShouldReturnBadRequest_WhenNameIsMissing()
        {
            // Arrange
            var invalidPerson = new { DisplayName = "John" };

            // Act
            var response = await _client.PostAsJsonAsync("api/v1/persons", invalidPerson);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var errorResponse = await response.Content.ReadAsStringAsync();
            errorResponse.Should().Contain("Name is required");
        }

        [Fact]
        public async Task CreatePerson_ShouldReturnBadRequest_WhenDisplayNameIsMissing()
        {
            // Arrange
            var invalidPerson = new { Name = "John Hick" };

            // Act
            var response = await _client.PostAsJsonAsync("api/v1/persons", invalidPerson);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var errorResponse = await response.Content.ReadAsStringAsync();
            errorResponse.Should().Contain("DisplayName is required");
        }

        [Fact]
        public async Task UpdatePerson_ShouldReturnNoContent_WhenPersonExist()
        {
            // Arrange
            var newPerson = new Person { Name = "Mike Doe", DisplayName = "Mike" };
            var createResponse = await _client.PostAsJsonAsync("api/v1/persons", newPerson);
            var createdPerson = await createResponse.Content.ReadFromJsonAsync<Person>();
            long personId = createdPerson.Id;

            var updatedPerson = new { Name = "Michael Doe", DisplayName = "Michael" };

            // Act
            var response = await _client.PutAsJsonAsync($"api/v1/persons/{personId}", updatedPerson);

            var checkResponse = await _client.GetAsync("api/v1/persons");
            var persons = await checkResponse.Content.ReadFromJsonAsync<List<Person>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            persons.Should().NotBeNull();
            persons[0].Name.Should().Be("Michael Doe");
            persons[0].DisplayName.Should().Be("Michael");

        }

        [Fact]
        public async Task UpdatePerson_ShouldReturnNotFound_WhenPersonDoesNotExist()
        {
            // Arrange
            long personId = 1;
            var updatedPerson = new Person { Name = "Mike Doe", DisplayName = "Mike" };

            // Act
            var response = await _client.PutAsJsonAsync($"api/v1/persons/{personId}", updatedPerson);

            var checkResponse = await _client.GetAsync("api/v1/persons");
            var persons = await checkResponse.Content.ReadFromJsonAsync<object>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            persons.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdatePerson_ShouldReturnBadRequest_WhenDataIsInvalid()
        {
            // Arrange
            var newPerson = new { Name = "Jane Doe", DisplayName = "Jane" };
            var createResponse = await _client.PostAsJsonAsync("api/v1/persons", newPerson);
            var createdPerson = await createResponse.Content.ReadFromJsonAsync<Person>();
            long personId = createdPerson.Id;

            var invalidUpdate = new { Name = "", DisplayName = "" };

            // Act
            var response = await _client.PutAsJsonAsync($"api/v1/persons/{personId}", invalidUpdate);

            var checkResponse = await _client.GetAsync("api/v1/persons");
            var persons = await checkResponse.Content.ReadFromJsonAsync<List<Person>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            persons[0].Name.Should().Be("Jane Doe");
            persons[0].DisplayName.Should().Be("Jane");
        }

        [Fact]
        public async Task DeletePerson_ShouldReturnNoContent_WhenPersonExists()
        {
            // Arrange
            var newPerson = new Person { Name = "Sarah Doe", DisplayName = "Sarah" };
            var createResponse = await _client.PostAsJsonAsync("api/v1/persons", newPerson);
            var createdPerson = await createResponse.Content.ReadFromJsonAsync<Person>();
            long personId = createdPerson.Id;

            // Act
            var response = await _client.DeleteAsync($"api/v1/persons/{personId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeletePerson_ShouldReturnNotFound_WhenPersonDoesNotExist()
        {
            // Arrange
            long personId = 1;

            // Act
            var response = await _client.DeleteAsync($"api/v1/persons/{personId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}