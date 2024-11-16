using System.Net;
using AspNetCoreWebApiSample.Models;
using AspNetCoreWebApiSample.Repositories.Documents;
using Microsoft.Azure.Cosmos;

namespace AspNetCoreWebApiSample.Repositories;

public class NoteRepository
{
    internal const string DatabaseName = "Notes";
    internal const string ContainerName = "Notes";

    private readonly CosmosClient _cosmosClient;
    private Container? _noteContainer;

    public NoteRepository(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
    }

    public async Task<Note> CreateNoteAsync(NewNote note, CancellationToken cancellationToken)
    {
        var noteContainer = _noteContainer ??= await CreateNoteContainerAsync(cancellationToken);

        var noteDocument = new NoteDocument
        {
            Id = Guid.NewGuid().ToString(),
            ETag = "",
            Text = note.Text,
        };

        var createResult = await noteContainer
            .CreateItemAsync(noteDocument, PartitionKey.Null, null, cancellationToken);

        return createResult.Resource.ToNote();
    }

    public async Task<Note?> GetNoteAsync(string id, CancellationToken cancellationToken)
    {
        var noteContainer = _noteContainer ??= await CreateNoteContainerAsync(cancellationToken);

        try
        {
            var getResult = await noteContainer
                .ReadItemAsync<NoteDocument>(id, PartitionKey.Null, null, cancellationToken);
            return getResult.Resource.ToNote();
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<Note> UpdateNoteAsync(Note note, CancellationToken cancellationToken)
    {
        var noteContainer = _noteContainer ??= await CreateNoteContainerAsync(cancellationToken);

        var noteDocument = new NoteDocument(note);

        var updateResult = await noteContainer
            .ReplaceItemAsync(noteDocument, noteDocument.Id, PartitionKey.Null,
                new ItemRequestOptions { IfMatchEtag = note.ETag }, cancellationToken);

        return updateResult.Resource.ToNote();
    }

    public async Task DeleteNoteAsync(string id, CancellationToken cancellationToken)
    {
        var noteContainer = _noteContainer ??= await CreateNoteContainerAsync(cancellationToken);

        _ = await noteContainer.DeleteItemAsync<NoteDocument>(id, PartitionKey.Null, null, cancellationToken);
    }

    private async Task<Container> CreateNoteContainerAsync(CancellationToken cancellationToken)
    {
        var databaseResponse = await _cosmosClient
            .CreateDatabaseIfNotExistsAsync(DatabaseName, cancellationToken: cancellationToken);
        var database = databaseResponse.Database;

        var containerResponse = await database
            .CreateContainerIfNotExistsAsync(new ContainerProperties(ContainerName, "/partitionKey"),
            cancellationToken: cancellationToken);
        
        return containerResponse.Container;
    }
}
