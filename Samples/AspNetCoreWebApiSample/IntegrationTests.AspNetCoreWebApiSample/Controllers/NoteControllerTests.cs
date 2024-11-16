using AspNetCoreWebApiSample.Controllers;
using AspNetCoreWebApiSample.Models;
using AspNetCoreWebApiSample.Repositories;
using CodeTiger.Azure.Cosmos.InMemory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;

namespace IntegrationTests.AspNetCoreWebApiSample.Controllers;

public class NoteControllerTests
{
    public class CreateNoteAsync
    {
        [Fact]
        public async Task CreatesNoteInDatabaseWhenNoteIsValid()
        {
            var cosmosClient = new InMemoryCosmosClient();
            var container = cosmosClient.GetContainer(NoteRepository.DatabaseName, NoteRepository.ContainerName);

            var target = CreateControllerForTesting(cosmosClient);

            var newNote = new NewNote { Text = Guid.NewGuid().ToString() };

            var noteResult = await target.CreateAsync(newNote, CancellationToken.None);
            var note = noteResult.Value;

            Assert.NotNull(note);

            var actual = await container.ReadItemAsync<Note>(note.Id, PartitionKey.Null);

            Assert.Equal(newNote.Text, actual.Resource.Text);
        }

        [Fact]
        public async Task ReturnsBadRequestErrorWhenNoteTextIsNull()
        {
            var cosmosClient = new InMemoryCosmosClient();

            var target = CreateControllerForTesting(cosmosClient);

            var newNote = new NewNote { Text = null! };

            var noteResult = await target.CreateAsync(newNote, CancellationToken.None);

            Assert.IsType<BadRequestObjectResult>(noteResult.Result);
        }

        [Fact]
        public async Task ReturnsBadRequestErrorWhenNoteTextIsEmpty()
        {
            var cosmosClient = new InMemoryCosmosClient();

            var target = CreateControllerForTesting(cosmosClient);

            var newNote = new NewNote { Text = "" };

            var noteResult = await target.CreateAsync(newNote, CancellationToken.None);

            Assert.IsType<BadRequestObjectResult>(noteResult.Result);
        }
    }

    public class GetAsync
    {
        [Fact]
        public async Task ReturnsExistingNote()
        {
            var cosmosClient = new InMemoryCosmosClient();
            var database = (await cosmosClient.CreateDatabaseAsync(NoteRepository.DatabaseName)).Database;
            var container = (await database
                .CreateContainerIfNotExistsAsync(NoteRepository.ContainerName, "/partitionKey"))
                .Container;

            string newNoteId = Guid.NewGuid().ToString();
            var newNote = (await container
                .CreateItemAsync(new Note { Id = newNoteId, Text = Guid.NewGuid().ToString() }, PartitionKey.Null))
                .Resource;

            var target = CreateControllerForTesting(cosmosClient);

            var noteResult = await target.GetAsync(newNote.Id, CancellationToken.None);
            
            var okResult = Assert.IsType<OkObjectResult>(noteResult.Result);

            var actual = Assert.IsType<Note>(okResult.Value);
            Assert.NotNull(actual);
            Assert.Equal(newNote.Id, actual.Id);
            Assert.Equal(newNote.Text, actual.Text);
        }

        [Fact]
        public async Task ReturnsNotFoundErrorWhenNoteDoesNotExist()
        {
            var cosmosClient = new InMemoryCosmosClient();

            var target = CreateControllerForTesting(cosmosClient);

            var noteResult = await target.GetAsync(Guid.NewGuid().ToString(), CancellationToken.None);

            Assert.IsType<NotFoundResult>(noteResult.Result);
        }
    }

    private static NoteController CreateControllerForTesting(InMemoryCosmosClient cosmosClient)
    {
        return new NoteController(new NoteRepository(cosmosClient));
    }
}
