using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;

namespace CodeTiger.Azure.Cosmos.InMemory;

/// <summary>
/// A proxy <see cref="Container"/> implementation that can be returned by operations immediately, without first
/// verifying that the container exists in the database (such as <see cref="Database.GetContainer(string)"/>).
/// </summary>
[SuppressMessage("CodeTiger.Reliability", "CT2001:Types with disposable state should implement IDisposable",
    Justification = "Instances of this class do not own the disposable objects it holds as state.")]
internal class InMemoryContainerProxy : Container
{
    private readonly InMemoryCosmosClient _inMemoryCosmosClient;
    private readonly InMemoryDatabaseProxy _inMemoryDatabaseProxy;

    public override string Id { get; }

    public override Database Database => _inMemoryDatabaseProxy;

    public override Conflicts Conflicts => throw new NotImplementedException();

    public override Scripts Scripts => throw new NotImplementedException();

    public InMemoryContainerProxy(InMemoryCosmosClient inMemoryCosmosClient,
        InMemoryDatabaseProxy inMemoryDatabaseProxy, string id)
    {
        _inMemoryCosmosClient = inMemoryCosmosClient;
        _inMemoryDatabaseProxy = inMemoryDatabaseProxy;

        Id = id;
    }

    public override Task<ItemResponse<T>> CreateItemAsync<T>(T item, PartitionKey? partitionKey = null,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallContainerAsync(
            x => x.CreateItemAsync(item, partitionKey, requestOptions, cancellationToken));
    }

    public override Task<ResponseMessage> CreateItemStreamAsync(Stream streamPayload, PartitionKey partitionKey,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallContainerAsync(
            x => x.CreateItemStreamAsync(streamPayload, partitionKey, requestOptions, cancellationToken));
    }

    public override TransactionalBatch CreateTransactionalBatch(PartitionKey partitionKey)
    {
        throw new NotImplementedException();
    }

    public override Task<ContainerResponse> DeleteContainerAsync(ContainerRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallContainerAsync(x => x.DeleteContainerAsync(requestOptions, cancellationToken));
    }

    public override Task<ResponseMessage> DeleteContainerStreamAsync(
        ContainerRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallContainerAsync(x => x.DeleteContainerStreamAsync(requestOptions, cancellationToken));
    }

    public override Task<ItemResponse<T>> DeleteItemAsync<T>(string id, PartitionKey partitionKey,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallContainerAsync(x => x.DeleteItemAsync<T>(id, partitionKey, requestOptions,
            cancellationToken));
    }

    public override Task<ResponseMessage> DeleteItemStreamAsync(string id, PartitionKey partitionKey,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallContainerAsync(
            x => x.DeleteItemStreamAsync(id, partitionKey, requestOptions, cancellationToken));
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
        return CallContainerAsync(x => x.GetFeedRangesAsync(cancellationToken));
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
        return CallContainerAsync(
            x => x.PatchItemAsync<T>(id, partitionKey, patchOperations, requestOptions, cancellationToken));
    }

    public override Task<ResponseMessage> PatchItemStreamAsync(string id, PartitionKey partitionKey,
        IReadOnlyList<PatchOperation> patchOperations, PatchItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallContainerAsync(
            x => x.PatchItemStreamAsync(id, partitionKey, patchOperations, requestOptions, cancellationToken));
    }

    public override Task<ContainerResponse> ReadContainerAsync(ContainerRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallContainerAsync(x => x.ReadContainerAsync(requestOptions, cancellationToken));
    }

    public override Task<ResponseMessage> ReadContainerStreamAsync(ContainerRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallContainerAsync(x => x.ReadContainerStreamAsync(requestOptions, cancellationToken));
    }

    public override Task<ItemResponse<T>> ReadItemAsync<T>(string id, PartitionKey partitionKey,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallContainerAsync(
            x => x.ReadItemAsync<T>(id, partitionKey, requestOptions, cancellationToken));
    }

    public override Task<ResponseMessage> ReadItemStreamAsync(string id, PartitionKey partitionKey,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallContainerAsync(x => x.ReadItemStreamAsync(id, partitionKey, requestOptions, cancellationToken));
    }

    public override Task<FeedResponse<T>> ReadManyItemsAsync<T>(
        IReadOnlyList<(string id, PartitionKey partitionKey)> items,
        ReadManyRequestOptions? readManyRequestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallContainerAsync(x => x.ReadManyItemsAsync<T>(items, readManyRequestOptions,
            cancellationToken));
    }

    public override Task<ResponseMessage> ReadManyItemsStreamAsync(
        IReadOnlyList<(string id, PartitionKey partitionKey)> items,
        ReadManyRequestOptions? readManyRequestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallContainerAsync(
            x => x.ReadManyItemsStreamAsync(items, readManyRequestOptions, cancellationToken));
    }

    public override Task<int?> ReadThroughputAsync(
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallContainerAsync(x => x.ReadThroughputAsync(cancellationToken));
    }

    public override Task<ThroughputResponse> ReadThroughputAsync(RequestOptions requestOptions,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallContainerAsync(x => x.ReadThroughputAsync(requestOptions, cancellationToken));
    }

    public override Task<ContainerResponse> ReplaceContainerAsync(ContainerProperties containerProperties,
        ContainerRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallContainerAsync(
            x => x.ReplaceContainerAsync(containerProperties, requestOptions, cancellationToken));
    }

    public override Task<ResponseMessage> ReplaceContainerStreamAsync(ContainerProperties containerProperties,
        ContainerRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallContainerAsync(
            x => x.ReplaceContainerStreamAsync(containerProperties, requestOptions, cancellationToken));
    }

    public override Task<ItemResponse<T>> ReplaceItemAsync<T>(T item, string id, PartitionKey? partitionKey = null,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallContainerAsync(x => x.ReplaceItemAsync(item, id, partitionKey, requestOptions,
            cancellationToken));
    }

    public override Task<ResponseMessage> ReplaceItemStreamAsync(Stream streamPayload, string id,
        PartitionKey partitionKey, ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallContainerAsync(
            x => x.ReplaceItemStreamAsync(streamPayload, id, partitionKey, requestOptions, cancellationToken));
    }

    public override Task<ThroughputResponse> ReplaceThroughputAsync(int throughput,
        RequestOptions? requestOptions = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallContainerAsync(x => x.ReplaceThroughputAsync(throughput, requestOptions, cancellationToken));
    }

    public override Task<ThroughputResponse> ReplaceThroughputAsync(ThroughputProperties throughputProperties,
        RequestOptions? requestOptions = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallContainerAsync(
            x => x.ReplaceThroughputAsync(throughputProperties, requestOptions, cancellationToken));
    }

    public override Task<ItemResponse<T>> UpsertItemAsync<T>(T item, PartitionKey? partitionKey = null,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallContainerAsync(
            x => x.UpsertItemAsync(item, partitionKey, requestOptions, cancellationToken));
    }

    public override Task<ResponseMessage> UpsertItemStreamAsync(Stream streamPayload, PartitionKey partitionKey,
        ItemRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallContainerAsync(
            x => x.UpsertItemStreamAsync(streamPayload, partitionKey, requestOptions, cancellationToken));
    }

    private Task<T> CallContainerAsync<T>(Func<Container, Task<T>> func)
    {
        if (!_inMemoryCosmosClient.InMemoryDatabases.TryGetValue(_inMemoryDatabaseProxy.Id, out var database))
        {
            string activityId = Guid.NewGuid().ToString();
            return Task.FromException<T>(new CosmosException(
                ExceptionMessageFactory.CreateDatabaseNotFound(activityId), HttpStatusCode.NotFound, 0, activityId,
                0));
        }

        if (!database.InMemoryContainers.TryGetValue(Id, out var container))
        {
            string activityId = Guid.NewGuid().ToString();
            return Task.FromException<T>(new CosmosException(
                ExceptionMessageFactory.CreateContainerNotFound(activityId), HttpStatusCode.NotFound, 0,
                activityId, 0));
        }

        return func(container);
    }
}
