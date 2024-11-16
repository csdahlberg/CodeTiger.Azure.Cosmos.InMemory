using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using AspNetCoreWebApiSample.Models;
using Newtonsoft.Json;

namespace AspNetCoreWebApiSample.Repositories.Documents;

public class NoteDocument
{
    [JsonProperty("partitionKey")]
    [JsonPropertyName("partitionKey")]
    public virtual string? PartitionKey => null;

    [JsonProperty("id")]
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonProperty("_etag")]
    [JsonPropertyName("_etag")]
    public required string ETag { get; init; }

    [JsonProperty("text")]
    [JsonPropertyName("text")]
    public required string Text { get; init; }

    public NoteDocument()
    {
    }

    [SetsRequiredMembers]
    public NoteDocument(Note note)
    {
        Id = note.Id;
        ETag = note.ETag;
        Text = note.Text;
    }

    public Note ToNote()
    {
        return new Note
        {
            Id = Id,
            ETag = ETag,
            Text = Text,
        };
    }
}
