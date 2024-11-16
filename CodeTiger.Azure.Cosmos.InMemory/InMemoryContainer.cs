using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CodeTiger.Azure.Cosmos.InMemory;

/// <summary>
/// An entirely in-memory implementation of <see cref="Container"/>.
/// </summary>
internal class InMemoryContainer : Container
{
    private static readonly Encoding _utf8NoBomEncoding = new UTF8Encoding(false, true);

    private readonly Func<JObject, PartitionKey> _defaultPartitionKeySelector;

    private long _nextDocumentRid = 1;

    public override string Id { get; }

    public override Database Database => InMemoryDatabase;

    public override Conflicts Conflicts => throw new NotImplementedException();

    public override Scripts Scripts => throw new NotImplementedException();

    internal ConcurrentDictionary<PartitionKey?, ConcurrentDictionary<string, JObject>> Partitions { get; }
        = new ConcurrentDictionary<PartitionKey?, ConcurrentDictionary<string, JObject>>();

    internal int Rid { get; }

    internal InMemoryDatabase InMemoryDatabase { get; }

    internal CosmosSerializer Serializer => InMemoryDatabase.InMemoryCosmosClient.Serializer;

    internal ContainerProperties Properties { get; }

    internal InMemoryContainer(int rid, InMemoryDatabase inMemoryDatabase, ContainerProperties properties)
    {
        Rid = rid;
        InMemoryDatabase = inMemoryDatabase;
        Id = properties.Id;

        Properties = properties;

        if (properties.PartitionKeyPaths?.Count > 1)
        {
            throw new NotImplementedException("Hierarchical partition keys are not currently supported.");
        }

        string? partitionKeyPath = properties.PartitionKeyPath;
        if (string.IsNullOrWhiteSpace(partitionKeyPath))
        {
            _defaultPartitionKeySelector = x => PartitionKey.Null;
        }
        else
        {
            string partitionKeyJsonPath = JsonPathHelper.CreateFromXPath(partitionKeyPath);
            _defaultPartitionKeySelector = x => new PartitionKey(x.SelectToken(partitionKeyJsonPath)?.ToString());
        }
    }

    public override Task<ItemResponse<T>> CreateItemAsync<T>(T item, PartitionKey? partitionKey = null,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        // Use the configured serializer to get the JSON representation as it would be stored in Cosmos, then
        // create a JSON.NET JObject from that to make it easier to manipulate (since System.Text.Json does not
        // support easy mutation until .NET 6.0 and this library needs to target netstandard2.0).
        JObject? itemJObject;
        using (var itemJsonStream = Serializer.ToStream(item))
        using (var streamReader = new StreamReader(itemJsonStream))
        using (var jsonTextReader = new JsonTextReader(streamReader))
        {
            var s = new JsonSerializer();
            itemJObject = s.Deserialize<JObject>(jsonTextReader);
        }

        if (itemJObject is null)
        {
            return Task.FromException<ItemResponse<T>>(
                new InvalidDataException($"{nameof(item)} serialized to a null JObject."));
        }

        string id;
        if (!itemJObject.TryGetValue("id", out var idToken))
        {
            SetNewId(itemJObject, out id, out idToken);
        }
        else if ((idToken.Type == JTokenType.String || idToken.Type == JTokenType.Guid)
            && idToken is JValue idValue)
        {
            if (idValue.Value is null)
            {
                SetNewId(itemJObject, out id, out idToken);
            }
            else
            {
                id = idValue.Value.ToString();
            }
        }
        else
        {
            throw new Exception($"{nameof(item)} had an 'id' property with an unexpected type of {idToken.Type}.");
        }

        if (!partitionKey.HasValue)
        {
            partitionKey = _defaultPartitionKeySelector(itemJObject);
        }

        var partition = Partitions.GetOrAdd(partitionKey.Value,
            key => new ConcurrentDictionary<string, JObject>());

        // It appears that Cosmos DB increments document RIDs by 4, so do the same here
        long itemRid = Interlocked.Add(ref _nextDocumentRid, 4);

        itemJObject["_rid"] = RidHelper.CreateRidString(InMemoryDatabase.Rid, Rid, itemRid);
        itemJObject["_self"] = RidHelper.CreateSelfLink(InMemoryDatabase.Rid, Rid, itemRid);
        itemJObject["_etag"] = "\"" + Guid.NewGuid().ToString() + "\"";
        itemJObject["_attachments"] = "attachments/";
        itemJObject["_ts"] = DateTimeOffset.Now.ToUnixTimeSeconds();

        if (!partition.TryAdd(id, itemJObject))
        {
            throw new CosmosException(
                "Response status code does not indicate success: 409 Substatus: 0 Reason: ().",
                HttpStatusCode.Conflict, 0, Guid.NewGuid().ToString(), 1.24);
        }

        using (var memoryStream = new MemoryStream())
        {
            using (var streamWriter = new StreamWriter(memoryStream, _utf8NoBomEncoding, 1024, leaveOpen: true))
            using (var jsonTextWriter = new JsonTextWriter(streamWriter))
            {
                itemJObject.WriteTo(jsonTextWriter);
            }

            memoryStream.Position = 0;
            item = Serializer.FromStream<T>(memoryStream);
        }

        return Task.FromResult<ItemResponse<T>>(new InMemoryItemResponse<T>(HttpStatusCode.Created, item));

        static void SetNewId(JObject itemJson, out string id, out JToken? idToken)
        {
            id = Guid.NewGuid().ToString();
            idToken = new JValue(id);
            itemJson["id"] = idToken;
        }
    }

    public override Task<ResponseMessage> CreateItemStreamAsync(Stream streamPayload, PartitionKey partitionKey,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override TransactionalBatch CreateTransactionalBatch(PartitionKey partitionKey)
    {
        throw new NotImplementedException();
    }

    public override Task<ContainerResponse> DeleteContainerAsync(ContainerRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        if (InMemoryDatabase.InMemoryContainers.TryRemove(Id, out var deletedContainer))
        {
            return Task.FromResult<ContainerResponse>(
                new InMemoryContainerResponse(deletedContainer, HttpStatusCode.OK, 1d));
        }
        else
        {
            string exceptionMessage = ExceptionMessageFactory.CreateContainerNotFound(Guid.NewGuid().ToString());
            return Task.FromException<ContainerResponse>(
                new CosmosException(exceptionMessage, HttpStatusCode.NotFound, 0,
                Guid.NewGuid().ToString(), 1d));
        }
    }

    public override Task<ResponseMessage> DeleteContainerStreamAsync(
        ContainerRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override Task<ItemResponse<T>> DeleteItemAsync<T>(string id, PartitionKey partitionKey,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        if (!Partitions.TryGetValue(partitionKey, out var partition))
        {
            string activityId = Guid.NewGuid().ToString();
            throw new CosmosException(ExceptionMessageFactory.CreatePartitionNotFound(activityId),
                HttpStatusCode.NotFound, 0, activityId, 0.0);
        }

        if (!partition.TryRemove(id, out var value))
        {
            string activityId = Guid.NewGuid().ToString();
            throw new CosmosException(ExceptionMessageFactory.CreateItemNotFound(activityId),
                HttpStatusCode.NotFound, 0, activityId, 0.0);
        }

        using (var itemJsonStream = new MemoryStream())
        {
            using (var itemJsonStreamWriter2
                = new StreamWriter(itemJsonStream, _utf8NoBomEncoding, 1024, leaveOpen: true))
            using (var itemJsonStreamWriter = new JsonTextWriter(itemJsonStreamWriter2))
            {
                value.WriteTo(itemJsonStreamWriter);
            }

            itemJsonStream.Position = 0;
            return Task.FromResult<ItemResponse<T>>(
                new InMemoryItemResponse<T>(HttpStatusCode.OK, Serializer.FromStream<T>(itemJsonStream)));
        }
    }

    public override Task<ResponseMessage> DeleteItemStreamAsync(string id, PartitionKey partitionKey,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override ChangeFeedEstimator GetChangeFeedEstimator(string processorName, Container leaseContainer)
    {
        throw new NotImplementedException();
    }

    public override ChangeFeedProcessorBuilder GetChangeFeedEstimatorBuilder(string processorName,
        ChangesEstimationHandler estimationDelegate, TimeSpan? estimationPeriod = null)
    {
        throw new NotImplementedException();
    }

    public override FeedIterator<T> GetChangeFeedIterator<T>(ChangeFeedStartFrom changeFeedStartFrom,
        ChangeFeedMode changeFeedMode, ChangeFeedRequestOptions? changeFeedRequestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilder<T>(string processorName,
        ChangesHandler<T> onChangesDelegate)
    {
        throw new NotImplementedException();
    }

    public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilder<T>(string processorName,
        ChangeFeedHandler<T> onChangesDelegate)
    {
        throw new NotImplementedException();
    }

    public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilder(string processorName,
        ChangeFeedStreamHandler onChangesDelegate)
    {
        throw new NotImplementedException();
    }

    public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilderWithManualCheckpoint<T>(
        string processorName, ChangeFeedHandlerWithManualCheckpoint<T> onChangesDelegate)
    {
        throw new NotImplementedException();
    }

    public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilderWithManualCheckpoint(
        string processorName, ChangeFeedStreamHandlerWithManualCheckpoint onChangesDelegate)
    {
        throw new NotImplementedException();
    }

    public override FeedIterator GetChangeFeedStreamIterator(ChangeFeedStartFrom changeFeedStartFrom,
        ChangeFeedMode changeFeedMode, ChangeFeedRequestOptions? changeFeedRequestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override Task<IReadOnlyList<FeedRange>> GetFeedRangesAsync(
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override IOrderedQueryable<T> GetItemLinqQueryable<T>(bool allowSynchronousQueryExecution = false,
        string? continuationToken = null, QueryRequestOptions? requestOptions = null,
        CosmosLinqSerializerOptions? linqSerializerOptions = null)
    {
        throw new NotImplementedException();
    }

    public override FeedIterator<T> GetItemQueryIterator<T>(QueryDefinition queryDefinition,
        string? continuationToken = null, QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override FeedIterator<T> GetItemQueryIterator<T>(string? queryText = null,
        string? continuationToken = null, QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override FeedIterator<T> GetItemQueryIterator<T>(FeedRange feedRange, QueryDefinition queryDefinition,
        string? continuationToken = null, QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override FeedIterator GetItemQueryStreamIterator(QueryDefinition queryDefinition,
        string? continuationToken = null, QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override FeedIterator GetItemQueryStreamIterator(string? queryText = null,
        string? continuationToken = null, QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override FeedIterator GetItemQueryStreamIterator(FeedRange feedRange, QueryDefinition queryDefinition,
        string continuationToken, QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override Task<ItemResponse<T>> PatchItemAsync<T>(string id, PartitionKey partitionKey,
        IReadOnlyList<PatchOperation> patchOperations, PatchItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override Task<ResponseMessage> PatchItemStreamAsync(string id, PartitionKey partitionKey,
        IReadOnlyList<PatchOperation> patchOperations, PatchItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override Task<ContainerResponse> ReadContainerAsync(ContainerRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override Task<ResponseMessage> ReadContainerStreamAsync(ContainerRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override Task<ItemResponse<T>> ReadItemAsync<T>(string id, PartitionKey partitionKey,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        if (!Partitions.TryGetValue(partitionKey, out var partition))
        {
            string activityId = Guid.NewGuid().ToString();
            throw new CosmosException(ExceptionMessageFactory.CreatePartitionNotFound(activityId),
                HttpStatusCode.NotFound, 0, activityId, 0.0);
        }

        if (!partition.TryGetValue(id, out var value))
        {
            string activityId = Guid.NewGuid().ToString();
            throw new CosmosException(ExceptionMessageFactory.CreateItemNotFound(activityId),
                HttpStatusCode.NotFound, 0, activityId, 0.0);
        }

        using (var itemJsonStream = new MemoryStream())
        {
            using (var itemJsonStreamWriter2
                = new StreamWriter(itemJsonStream, _utf8NoBomEncoding, 1024, leaveOpen: true))
            using (var itemJsonStreamWriter = new JsonTextWriter(itemJsonStreamWriter2))
            {
                value.WriteTo(itemJsonStreamWriter);
            }

            itemJsonStream.Position = 0;
            return Task.FromResult<ItemResponse<T>>(
                new InMemoryItemResponse<T>(HttpStatusCode.OK, Serializer.FromStream<T>(itemJsonStream)));
        }
    }

    public override Task<ResponseMessage> ReadItemStreamAsync(string id, PartitionKey partitionKey,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override Task<FeedResponse<T>> ReadManyItemsAsync<T>(
        IReadOnlyList<(string id, PartitionKey partitionKey)> items,
        ReadManyRequestOptions? readManyRequestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override Task<ResponseMessage> ReadManyItemsStreamAsync(
        IReadOnlyList<(string id, PartitionKey partitionKey)> items,
        ReadManyRequestOptions? readManyRequestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override Task<int?> ReadThroughputAsync(
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override Task<ThroughputResponse> ReadThroughputAsync(RequestOptions requestOptions,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override Task<ContainerResponse> ReplaceContainerAsync(ContainerProperties containerProperties,
        ContainerRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override Task<ResponseMessage> ReplaceContainerStreamAsync(ContainerProperties containerProperties,
        ContainerRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override Task<ItemResponse<T>> ReplaceItemAsync<T>(T item, string id, PartitionKey? partitionKey = null,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        if (!Partitions.TryGetValue(partitionKey, out var partition))
        {
            string activityId = Guid.NewGuid().ToString();
            throw new CosmosException(ExceptionMessageFactory.CreatePartitionNotFound(activityId),
                HttpStatusCode.NotFound, 0, activityId, 0.0);
        }

        if (!partition.TryRemove(id, out var value))
        {
            string activityId = Guid.NewGuid().ToString();
            throw new CosmosException(ExceptionMessageFactory.CreateItemNotFound(activityId),
                HttpStatusCode.NotFound, 0, activityId, 0.0);
        }

        using (var itemJsonStream = new MemoryStream())
        {
            using (var itemJsonStreamWriter2
                = new StreamWriter(itemJsonStream, _utf8NoBomEncoding, 1024, leaveOpen: true))
            using (var itemJsonStreamWriter = new JsonTextWriter(itemJsonStreamWriter2))
            {
                value.WriteTo(itemJsonStreamWriter);
            }

            itemJsonStream.Position = 0;
            return Task.FromResult<ItemResponse<T>>(
                new InMemoryItemResponse<T>(HttpStatusCode.OK, Serializer.FromStream<T>(itemJsonStream)));
        }
    }

    public override Task<ResponseMessage> ReplaceItemStreamAsync(Stream streamPayload, string id,
        PartitionKey partitionKey, ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override Task<ThroughputResponse> ReplaceThroughputAsync(int throughput,
        RequestOptions? requestOptions = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override Task<ThroughputResponse> ReplaceThroughputAsync(ThroughputProperties throughputProperties,
        RequestOptions? requestOptions = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override Task<ItemResponse<T>> UpsertItemAsync<T>(T item, PartitionKey? partitionKey = null,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        var partition = Partitions.GetOrAdd(partitionKey, _ => new ConcurrentDictionary<string, JObject>());

        if (!partition.TryRemove(id, out var value))
        {
            string activityId = Guid.NewGuid().ToString();
            throw new CosmosException(ExceptionMessageFactory.CreateItemNotFound(activityId),
                HttpStatusCode.NotFound, 0, activityId, 0.0);
        }

        using (var itemJsonStream = new MemoryStream())
        {
            using (var itemJsonStreamWriter2
                = new StreamWriter(itemJsonStream, _utf8NoBomEncoding, 1024, leaveOpen: true))
            using (var itemJsonStreamWriter = new JsonTextWriter(itemJsonStreamWriter2))
            {
                value.WriteTo(itemJsonStreamWriter);
            }

            itemJsonStream.Position = 0;
            return Task.FromResult<ItemResponse<T>>(
                new InMemoryItemResponse<T>(HttpStatusCode.OK, Serializer.FromStream<T>(itemJsonStream)));
        }
    }

    public override Task<ResponseMessage> UpsertItemStreamAsync(Stream streamPayload, PartitionKey partitionKey,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }
}
