using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;

namespace CodeTiger.Azure.Cosmos.InMemory;

/// <summary>
/// A proxy <see cref="Database"/> implementation that can be returned by operations immediately, without first
/// verifying that the database exists in the account (such as <see cref="CosmosClient.GetDatabase(string)"/>).
/// </summary>
[SuppressMessage("CodeTiger.Reliability", "CT2001:Types with disposable state should implement IDisposable.",
    Justification = "CosmosClient is disposable, but this class should not be responsible for disposing it.")]
internal class InMemoryDatabaseProxy : Database
{
    private readonly InMemoryCosmosClient _inMemoryCosmosClient;
    
    public override string Id { get; }

    public override CosmosClient Client => _inMemoryCosmosClient;

    public InMemoryDatabaseProxy(string id, InMemoryCosmosClient inMemoryCosmosClient)
    {
        Id = id;
        _inMemoryCosmosClient = inMemoryCosmosClient;
    }

    public override Task<ClientEncryptionKeyResponse> CreateClientEncryptionKeyAsync(
        ClientEncryptionKeyProperties clientEncryptionKeyProperties,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallDatabaseAsync(
            x => x.CreateClientEncryptionKeyAsync(clientEncryptionKeyProperties, requestOptions,
                cancellationToken));
    }

    public override Task<ContainerResponse> CreateContainerAsync(ContainerProperties containerProperties,
        ThroughputProperties throughputProperties, RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallDatabaseAsync(
            x => x.CreateContainerAsync(containerProperties, throughputProperties, requestOptions,
                cancellationToken));
    }

    public override Task<ContainerResponse> CreateContainerAsync(ContainerProperties containerProperties,
        int? throughput = null, RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallDatabaseAsync(
            x => x.CreateContainerAsync(containerProperties, throughput, requestOptions, cancellationToken));
    }

    public override Task<ContainerResponse> CreateContainerAsync(string id, string partitionKeyPath,
        int? throughput = null, RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallDatabaseAsync(
            x => x.CreateContainerAsync(id, partitionKeyPath, throughput, requestOptions, cancellationToken));
    }

    public override Task<ContainerResponse> CreateContainerIfNotExistsAsync(
        ContainerProperties containerProperties, ThroughputProperties throughputProperties,
        RequestOptions? requestOptions = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallDatabaseAsync(
            x => x.CreateContainerIfNotExistsAsync(containerProperties, throughputProperties, requestOptions,
                cancellationToken));
    }

    public override Task<ContainerResponse> CreateContainerIfNotExistsAsync(
        ContainerProperties containerProperties, int? throughput = null, RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallDatabaseAsync(
            x => x.CreateContainerIfNotExistsAsync(containerProperties, throughput, requestOptions,
                cancellationToken));
    }

    public override Task<ContainerResponse> CreateContainerIfNotExistsAsync(string id, string partitionKeyPath,
        int? throughput = null, RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallDatabaseAsync(
            x => x.CreateContainerIfNotExistsAsync(id, partitionKeyPath, throughput, requestOptions,
                cancellationToken));
    }

    public override Task<ResponseMessage> CreateContainerStreamAsync(ContainerProperties containerProperties,
        ThroughputProperties throughputProperties, RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallDatabaseAsync(
            x => x.CreateContainerStreamAsync(containerProperties, throughputProperties, requestOptions,
                cancellationToken));
    }

    public override Task<ResponseMessage> CreateContainerStreamAsync(ContainerProperties containerProperties,
        int? throughput = null, RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallDatabaseAsync(
            x => x.CreateContainerStreamAsync(containerProperties, throughput, requestOptions, cancellationToken));
    }

    public override Task<UserResponse> CreateUserAsync(string id, RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallDatabaseAsync(x => x.CreateUserAsync(id, requestOptions, cancellationToken));
    }

    public override ContainerBuilder DefineContainer(string name, string partitionKeyPath)
    {
        return new ContainerBuilder(this, name, partitionKeyPath);
    }

    public override Task<DatabaseResponse> DeleteAsync(RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallDatabaseAsync(x => x.DeleteAsync(requestOptions, cancellationToken));
    }

    public override Task<ResponseMessage> DeleteStreamAsync(RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallDatabaseAsync(x => x.DeleteStreamAsync(requestOptions, cancellationToken));
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
        Guard.ArgumentIsNotNullOrEmpty(nameof(id), id);

        return new InMemoryContainerProxy(_inMemoryCosmosClient, this, id);
    }

    public override FeedIterator<T> GetContainerQueryIterator<T>(QueryDefinition? queryDefinition,
        string? continuationToken = null, QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override FeedIterator<T> GetContainerQueryIterator<T>(string? queryText = null,
        string? continuationToken = null, QueryRequestOptions? requestOptions = null)
    {
        var queryDefinition = queryText is not null ? new QueryDefinition(queryText) : null;

        return GetContainerQueryIterator<T>(queryDefinition, continuationToken, requestOptions);
    }

    public override FeedIterator GetContainerQueryStreamIterator(QueryDefinition? queryDefinition,
        string? continuationToken = null, QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override FeedIterator GetContainerQueryStreamIterator(string? queryText = null,
        string? continuationToken = null, QueryRequestOptions? requestOptions = null)
    {
        var queryDefinition = queryText is not null ? new QueryDefinition(queryText) : null;

        return GetContainerQueryStreamIterator(queryDefinition, continuationToken, requestOptions);
    }

    public override User GetUser(string id)
    {
        throw new NotImplementedException();
    }

    public override FeedIterator<T> GetUserQueryIterator<T>(string? queryText = null,
        string? continuationToken = null, QueryRequestOptions? requestOptions = null)
    {
        var queryDefinition = queryText is not null ? new QueryDefinition(queryText) : null;

        return GetUserQueryIterator<T>(queryDefinition, continuationToken, requestOptions);
    }

    public override FeedIterator<T> GetUserQueryIterator<T>(QueryDefinition? queryDefinition,
        string? continuationToken = null, QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    public override Task<DatabaseResponse> ReadAsync(RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallDatabaseAsync(x => x.ReadAsync(requestOptions, cancellationToken));
    }

    public override Task<ResponseMessage> ReadStreamAsync(RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallDatabaseAsync(x => x.ReadStreamAsync(requestOptions, cancellationToken));
    }

    public override Task<int?> ReadThroughputAsync(
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallDatabaseAsync(x => x.ReadThroughputAsync(cancellationToken));
    }

    public override Task<ThroughputResponse> ReadThroughputAsync(RequestOptions requestOptions,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallDatabaseAsync(x => x.ReadThroughputAsync(requestOptions, cancellationToken));
    }

    public override Task<ThroughputResponse> ReplaceThroughputAsync(ThroughputProperties throughputProperties,
        RequestOptions? requestOptions = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallDatabaseAsync(
            x => x.ReplaceThroughputAsync(throughputProperties, requestOptions, cancellationToken));
    }

    public override Task<ThroughputResponse> ReplaceThroughputAsync(int throughput,
        RequestOptions? requestOptions = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallDatabaseAsync(x => x.ReplaceThroughputAsync(throughput, requestOptions, cancellationToken));
    }

    public override Task<UserResponse> UpsertUserAsync(string id, RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return CallDatabaseAsync(x => x.UpsertUserAsync(id, requestOptions, cancellationToken));
    }

    private Task<T> CallDatabaseAsync<T>(Func<Database, Task<T>> func)
    {
        if (!_inMemoryCosmosClient.InMemoryDatabases.TryGetValue(Id, out var database))
        {
            string activityId = Guid.NewGuid().ToString();
            return Task.FromException<T>(new CosmosException(
                ExceptionMessageFactory.CreateDatabaseNotFound(activityId), HttpStatusCode.NotFound, 0, activityId,
                0));
        }

        return func(database);
    }
}
