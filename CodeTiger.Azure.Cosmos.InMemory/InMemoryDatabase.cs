using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;

namespace CodeTiger.Azure.Cosmos.InMemory;

/// <summary>
/// An entirely in-memory implementation of <see cref="Database"/>.
/// </summary>
[SuppressMessage("CodeTiger.Reliability", "CT2001:Types with disposable state should implement IDisposable",
    Justification = "Instances of this class do not own the disposable objects it holds as state.")]
internal class InMemoryDatabase : Database
{
    private static int _nextRid = new Random().Next(0x01000000, 0x70000000);

    private int _nextContainerRid = new Random().Next(0x01000000, 0x70000000);

    public override CosmosClient Client => InMemoryCosmosClient;

    public override string Id { get; }

    public InMemoryCosmosClient InMemoryCosmosClient { get; }

    internal int Rid { get; } = Interlocked.Increment(ref _nextRid);

    internal ConcurrentDictionary<string, InMemoryContainer> InMemoryContainers { get; }
        = new ConcurrentDictionary<string, InMemoryContainer>();

    public InMemoryDatabase(InMemoryCosmosClient inMemoryCosmosClient, string id)
    {
        InMemoryCosmosClient = inMemoryCosmosClient;
        Id = id;
    }

    public override Task<ClientEncryptionKeyResponse> CreateClientEncryptionKeyAsync(
        ClientEncryptionKeyProperties clientEncryptionKeyProperties, RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override Task<ContainerResponse> CreateContainerAsync(ContainerProperties containerProperties,
        ThroughputProperties? throughputProperties, RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        var newContainer = new InMemoryContainer(Interlocked.Increment(ref _nextContainerRid), this,
            containerProperties);

        if (!InMemoryContainers.TryAdd(containerProperties.Id, newContainer))
        {
            string activityId = Guid.NewGuid().ToString();

            return Task.FromException<ContainerResponse>(new CosmosException(
                ExceptionMessageFactory.CreateContainerExists(activityId), HttpStatusCode.Conflict, 0, activityId,
                0));
        }

        return Task.FromResult<ContainerResponse>(new InMemoryContainerResponse(newContainer,
            HttpStatusCode.Created, 1));
    }

    public override Task<ContainerResponse> CreateContainerAsync(ContainerProperties containerProperties,
        int? throughput = null, RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        var throughputProperties = throughput.HasValue
            ? ThroughputProperties.CreateManualThroughput(throughput.Value)
            : null;

        return CreateContainerAsync(containerProperties, throughputProperties, requestOptions, cancellationToken);
    }

    public override Task<ContainerResponse> CreateContainerAsync(string id, string partitionKeyPath,
        int? throughput = null, RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        var containerProperties = new ContainerProperties(id, partitionKeyPath);
        var throughputProperties = throughput.HasValue
            ? ThroughputProperties.CreateManualThroughput(throughput.Value)
            : null;

        return CreateContainerAsync(containerProperties, throughputProperties, requestOptions, cancellationToken);
    }

    public override Task<ContainerResponse> CreateContainerIfNotExistsAsync(
        ContainerProperties containerProperties, ThroughputProperties? throughputProperties,
        RequestOptions? requestOptions = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        var newContainer = new InMemoryContainer(Interlocked.Increment(ref _nextContainerRid), this,
            containerProperties);

        var container = InMemoryContainers.GetOrAdd(containerProperties.Id, newContainer);

        return Task.FromResult<ContainerResponse>(new InMemoryContainerResponse(container, HttpStatusCode.Created,
            1));
    }

    public override Task<ContainerResponse> CreateContainerIfNotExistsAsync(
        ContainerProperties containerProperties, int? throughput = null, RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        var throughputProperties = throughput.HasValue
            ? ThroughputProperties.CreateManualThroughput(throughput.Value)
            : null;

        return CreateContainerIfNotExistsAsync(containerProperties, throughputProperties, requestOptions,
            cancellationToken);
    }

    public override Task<ContainerResponse> CreateContainerIfNotExistsAsync(string id, string partitionKeyPath,
        int? throughput = null, RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        var containerProperties = new ContainerProperties(id, partitionKeyPath);
        var throughputProperties = throughput.HasValue
            ? ThroughputProperties.CreateManualThroughput(throughput.Value)
            : null;

        return CreateContainerIfNotExistsAsync(containerProperties, throughputProperties, requestOptions,
            cancellationToken);
    }

    public override Task<ResponseMessage> CreateContainerStreamAsync(ContainerProperties containerProperties,
        ThroughputProperties throughputProperties, RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override Task<ResponseMessage> CreateContainerStreamAsync(ContainerProperties containerProperties,
        int? throughput = null, RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override Task<UserResponse> CreateUserAsync(string id, RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override ContainerBuilder DefineContainer(string name, string partitionKeyPath)
    {
        throw new NotImplementedException();
    }

    public override Task<DatabaseResponse> DeleteAsync(RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        if (!InMemoryCosmosClient.InMemoryDatabases.TryRemove(Id, out _))
        {
            return Task.FromException<DatabaseResponse>(new CosmosException(
                "Response status code does not indicate success: NotFound (404)", HttpStatusCode.NotFound, 0,
                Guid.NewGuid().ToString(), 2));
        }

        return Task.FromResult<DatabaseResponse>(
            new InMemoryDatabaseResponse(this, HttpStatusCode.NoContent, 4.95));
    }

    public override Task<ResponseMessage> DeleteStreamAsync(RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override ClientEncryptionKey GetClientEncryptionKey(string id)
    {
        throw new NotImplementedException();
    }

    public override FeedIterator<ClientEncryptionKeyProperties> GetClientEncryptionKeyQueryIterator(
        QueryDefinition queryDefinition, string? continuationToken = null,
        QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override Container GetContainer(string id)
    {
        return new InMemoryContainerProxy(InMemoryCosmosClient,
            new InMemoryDatabaseProxy(Id, InMemoryCosmosClient), id);
    }

    public override FeedIterator<T> GetContainerQueryIterator<T>(QueryDefinition queryDefinition,
        string? continuationToken = null, QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override FeedIterator<T> GetContainerQueryIterator<T>(string? queryText = null,
        string? continuationToken = null, QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override FeedIterator GetContainerQueryStreamIterator(QueryDefinition queryDefinition,
        string? continuationToken = null, QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override FeedIterator GetContainerQueryStreamIterator(string? queryText = null,
        string? continuationToken = null, QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override User GetUser(string id)
    {
        throw new NotImplementedException();
    }

    public override FeedIterator<T> GetUserQueryIterator<T>(string? queryText = null,
        string? continuationToken = null, QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override FeedIterator<T> GetUserQueryIterator<T>(QueryDefinition queryDefinition,
        string? continuationToken = null, QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override Task<DatabaseResponse> ReadAsync(RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override Task<ResponseMessage> ReadStreamAsync(RequestOptions? requestOptions = null,
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

    public override Task<ThroughputResponse> ReplaceThroughputAsync(ThroughputProperties throughputProperties,
        RequestOptions? requestOptions = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override Task<ThroughputResponse> ReplaceThroughputAsync(int throughput,
        RequestOptions? requestOptions = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }

    public override Task<UserResponse> UpsertUserAsync(string id, RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        throw new NotImplementedException();
    }
}
