using System.IO;
using System.Text;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CodeTiger.Azure.Cosmos.InMemory;

/// <summary>
/// A <see cref="CosmosSerializer"/> implementation that mirrors the behavior of the official (though internal)
/// CosmosJsonDotNetSerializer class.
/// </summary>
internal sealed class InMemoryCosmosJsonDotNetSerializer : CosmosSerializer
{
    private static readonly Encoding _utf8NoBomEncoding = new UTF8Encoding(false, true);

    private readonly JsonSerializerSettings? _serializerSettings;

    public InMemoryCosmosJsonDotNetSerializer()
    {
    }

    public InMemoryCosmosJsonDotNetSerializer(CosmosSerializationOptions? options)
    {
        _serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = options?.IgnoreNullValues == true
                ? NullValueHandling.Ignore
                : NullValueHandling.Include,
            Formatting = options?.Indented == true
                ? Formatting.Indented
                : Formatting.None,
            ContractResolver = options?.PropertyNamingPolicy == CosmosPropertyNamingPolicy.CamelCase
                ? new CamelCasePropertyNamesContractResolver()
                : null,
        };
    }

    public override T FromStream<T>(Stream stream)
    {
        using (stream)
        {
            // This seems a bit odd, but it mirrors the behavior of the official CosmosJsonDotNetSerializer class
            if (typeof(Stream).IsAssignableFrom(typeof(T)))
            {
                return (T)(object)stream;
            }

            using (var streamReader = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(streamReader))
            {
                var jsonSerializer = JsonSerializer.Create(_serializerSettings);
                var item = jsonSerializer.Deserialize<T>(jsonTextReader);

                if (item is null)
                {
                    throw new InvalidDataException("Deserializing the stream resulted in a null value.");
                }

                return item;
            }
        }
    }

    public override Stream ToStream<T>(T input)
    {
        var streamPayload = new MemoryStream();

        using (var streamWriter = new StreamWriter(streamPayload, _utf8NoBomEncoding, 1024, true))
        using (var writer = new JsonTextWriter(streamWriter))
        {
            writer.Formatting = Formatting.None;
            var jsonSerializer = JsonSerializer.Create(_serializerSettings);
            jsonSerializer.Serialize(writer, input);
            writer.Flush();
            streamWriter.Flush();
        }

        streamPayload.Position = 0;
        return streamPayload;
    }
}
