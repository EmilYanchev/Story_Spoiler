using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Text.Json;
using StorySpoiler.Models;

namespace StorySpoiler
{
    [TestFixture]
    public class ApiTests 
    {
        private RestClient _client;
        private static string lastCreatedStoryId;
        private static string fakeStoryId = "12341";
        private static string baseUrl = "https://d3s5nxhwblsjbi.cloudfront.net/";

        [OneTimeSetUp]
        public void Setup()
        {
            string token = GetJwtToken("eyanch", "emil@1998");
            var options = new RestClientOptions(baseUrl)
            {
                Authenticator = new JwtAuthenticator(token)
            };
            _client = new RestClient(options); 
        }

        private string GetJwtToken(string username, string password)
        {
            var loginClient = new RestClient(baseUrl);
            var request = new RestRequest("/api/User/Authentication", Method.Post);
            request.AddJsonBody(new { username, password });
            var response = loginClient.Execute(request);
            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
            return json.GetProperty("accessToken").GetString() ?? string.Empty;
        }

        [Test, Order(1)]
        public void CreateNewStorySpoiler_WithRequiredFields()
        {
            //Arrange & Act
            var story = new StoryDto
            {
                Title = "Test Story",
                Description = "Test Story Description",
                Url = ""
            };
            var request = new RestRequest("/api/Story/Create", Method.Post);
            request.AddJsonBody(story);
            var response = _client.Execute(request);
            
            //Assert
            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(json.GetProperty("storyId").GetString(), Is.Not.Empty);
            Assert.That(json.GetProperty("msg").GetString(), Is.EqualTo("Successfully created!"));
            lastCreatedStoryId = json.GetProperty("storyId").GetString();
            
        }

        [Test, Order(2)]
        public void EditTheCreatedStorySpoiler()
        {
            //Arrange & Act
            var editRequest = new StoryDto()
            {
                Title = "Edited Story",
                Description = "Edited Story Description",
                Url = ""
            };
            var request = new RestRequest($"/api/Story/Edit/{lastCreatedStoryId}", Method.Put);
            request.AddJsonBody(editRequest);
            var response = _client.Execute(request);
            
            //Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
            Assert.That(json.GetProperty("msg").GetString(), Is.EqualTo("Successfully edited"));
        }

        [Test, Order(3)]
        public void GetAllStorySpoilers()
        {
            //Arrange & Act
            var request = new RestRequest($"/api/Story/All", Method.Get);
            var response = _client.Execute(request);
            //Assert
            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Content, Is.Not.Null);
            Assert.That(json.ValueKind, Is.EqualTo(JsonValueKind.Array), "Response is an array.");
            Assert.That(json.GetArrayLength(), Is.GreaterThan(0), "Array is not empty.");
        }

        [Test, Order(4)]
        public void DeleteStorySpoiler()
        {
            //Arrange & Act
            var request = new RestRequest($"/api/Story/Delete/{lastCreatedStoryId}", Method.Delete);
            var response = _client.Execute(request);
            //Assert
            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(json.GetProperty("msg").GetString(), Is.EqualTo("Deleted successfully!"));
        }

        [Test, Order(5)]
        public void CreateStorySpoiler_WithoutRequiredFields()
        {
            //Arrange & Act
            var story = new StoryDto
            {
                Title = "",
                Description = "",
                Url = ""
            };
            var request = new RestRequest("/api/Story/Create", Method.Post);
            request.AddJsonBody(story);
            var response = _client.Execute(request);
            
            //Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test, Order(6)]
        public void EditStorySpoiler_WithNonExistingStoryId()
        {
            //Arrange & Act
            var editRequest = new StoryDto()
            {
                Title = "Edited Story",
                Description = "Edited Story Description",
                Url = ""
            };
            var request = new RestRequest($"/api/Story/Edit/{fakeStoryId}", Method.Put);
            request.AddJsonBody(editRequest);
            var response = _client.Execute(request);
            
            //Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
            Assert.That(json.GetProperty("msg").GetString(), Is.EqualTo("No spoilers..."));
        }

        [Test, Order(7)]
        public void DeleteStorySpoiler_WithoutExistingStoryId()
        {
            //Arrange & Act
            var request = new RestRequest($"/api/Story/Delete/{fakeStoryId}", Method.Delete);
            var response = _client.Execute(request);
            //Assert
            var json = JsonSerializer.Deserialize<JsonElement>(response.Content);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(json.GetProperty("msg").GetString(), Is.EqualTo("Unable to delete this story spoiler!"));
        }
        
        [OneTimeTearDown]
        public void Cleanup()
        {
            _client?.Dispose();
        }
    }
}