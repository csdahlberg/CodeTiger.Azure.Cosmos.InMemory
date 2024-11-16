namespace AspNetCoreWebApiSample.Models;

public class Note
{
    public string Id { get; init; } = "";

    public string ETag { get; init; } = "";

    public required string Text { get; init; }
}
