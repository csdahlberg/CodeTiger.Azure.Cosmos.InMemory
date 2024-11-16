using System;
using System.Collections.Concurrent;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace CodeTiger.Azure.Cosmos.InMemory;

/// <summary>
/// An entirely in-memory implementation of <see cref="CosmosClient"/> that can be used for testing purposes.
/// </summary>
public class InMemoryCosmosClient : CosmosClient
{
    /// <summary>
    /// The default account name used for in-memory Cosmos databases when no account name is specified.
    /// </summary>
    public static readonly string DefaultAccountName = "InMemory";

    /// <summary>
    /// Data shared by all <see cref="InMemoryCosmosClient"/> instances when the "shouldUseSharedData" constructor
    /// argument is set to <c>true</c>.
    /// </summary>
    private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, InMemoryDatabase>>
        _sharedAccounts = new ConcurrentDictionary<string, ConcurrentDictionary<string, InMemoryDatabase>>();

    internal ConcurrentDictionary<string, ConcurrentDictionary<string, InMemoryDatabase>> Accounts { get; }

    internal CosmosSerializer Serializer { get; }

    internal ConcurrentDictionary<string, InMemoryDatabase> InMemoryDatabases { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryCosmosClient"/> class with default settings.
    /// </summary>
    public InMemoryCosmosClient()
        : this(DefaultAccountName)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryCosmosClient"/> class with default settings.
    /// </summary>
    /// <param name="shouldUseSharedData"></param>
    public InMemoryCosmosClient(bool shouldUseSharedData)
        : this(DefaultAccountName, shouldUseSharedData)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryCosmosClient"/> class with a specified account name
    /// and default client options.
    /// </summary>
    /// <param name="accountName"></param>
    public InMemoryCosmosClient(string accountName)
        : this(accountName, null, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryCosmosClient"/> class with a specified account name
    /// and default client options.
    /// </summary>
    /// <param name="accountName"></param>
    /// <param name="shouldUseSharedData"></param>
    public InMemoryCosmosClient(string accountName, bool shouldUseSharedData)
        : this(accountName, null, shouldUseSharedData)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryCosmosClient"/> class with a specified account name
    /// and default client options.
    /// </summary>
    /// <param name="accountName"></param>
    /// <param name="clientOptions"></param>
    public InMemoryCosmosClient(string accountName, CosmosClientOptions? clientOptions)
        : this(accountName, clientOptions, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryCosmosClient"/> class with specified client options
    /// and a default account name.
    /// </summary>
    /// <param name="clientOptions"></param>
    public InMemoryCosmosClient(CosmosClientOptions? clientOptions)
        : this(DefaultAccountName, clientOptions, false)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryCosmosClient"/> class with specified client options
    /// and a default account name.
    /// </summary>
    /// <param name="clientOptions"></param>
    /// <param name="shouldUseSharedData"></param>
    public InMemoryCosmosClient(CosmosClientOptions? clientOptions, bool shouldUseSharedData)
        : this(DefaultAccountName, clientOptions, shouldUseSharedData)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InMemoryCosmosClient"/> class with a specified account name
    /// and client options.
    /// </summary>
    /// <param name="accountName"></param>
    /// <param name="clientOptions"></param>
    /// <param name="shouldUseSharedData"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public InMemoryCosmosClient(string accountName, CosmosClientOptions? clientOptions, bool shouldUseSharedData)
        : base()
    {
        if (accountName is null)
        {
            throw new ArgumentNullException(nameof(accountName), "The account name is required.");
        }

        if (string.IsNullOrEmpty(accountName))
        {
            throw new ArgumentException("The account name is required.", nameof(accountName));
        }

        Serializer = clientOptions?.Serializer is not null
            ? clientOptions.Serializer
            : new InMemoryCosmosJsonDotNetSerializer(clientOptions?.SerializerOptions);

        Accounts = shouldUseSharedData
            ? _sharedAccounts
            : new ConcurrentDictionary<string, ConcurrentDictionary<string, InMemoryDatabase>>();
        InMemoryDatabases = Accounts.GetOrAdd(accountName,
            _ => new ConcurrentDictionary<string, InMemoryDatabase>());
    }

    /// <inheritdoc/>
    public override Task<DatabaseResponse> CreateDatabaseAsync(string id, int? throughput = null,
        RequestOptions? requestOptions = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        Guard.ArgumentIsNotNullOrEmpty(nameof(id), id);

        if (requestOptions is not null)
        {
            throw new NotImplementedException($"A non-null '{nameof(requestOptions)}' argument is not handled.");
        }

        var newDatabase = new InMemoryDatabase(this, id);
        var database = InMemoryDatabases.GetOrAdd(id, newDatabase);

        if (!ReferenceEquals(newDatabase, database))
        {
            string activityId = Guid.NewGuid().ToString();

            return Task.FromException<DatabaseResponse>(new CosmosException(
                ExceptionMessageFactory.CreateDatabaseExists(activityId), HttpStatusCode.Conflict, 0, activityId,
                1.24));
        }

        return Task.FromResult<DatabaseResponse>(
            new InMemoryDatabaseResponse(database, HttpStatusCode.Created, 4.95));
    }

    /// <inheritdoc/>
    public override Task<DatabaseResponse> CreateDatabaseAsync(string id,
        ThroughputProperties? throughputProperties, RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        Guard.ArgumentIsNotNullOrEmpty(nameof(id), id);

        if (requestOptions is not null)
        {
            throw new NotImplementedException($"A non-null '{nameof(requestOptions)}' argument is not handled.");
        }

        var newDatabase = new InMemoryDatabase(this, id);
        var database = InMemoryDatabases.GetOrAdd(id, newDatabase);

        if (!ReferenceEquals(newDatabase, database))
        {
            string activityId = Guid.NewGuid().ToString();

            return Task.FromException<DatabaseResponse>(new CosmosException(
                ExceptionMessageFactory.CreateDatabaseExists(activityId), HttpStatusCode.Conflict, 0, activityId,
                1.24));
        }

        return Task.FromResult<DatabaseResponse>(
            new InMemoryDatabaseResponse(database, HttpStatusCode.Created, 4.95));
    }

    /// <inheritdoc/>
    public override Task<DatabaseResponse> CreateDatabaseIfNotExistsAsync(string id, int? throughput = null,
        RequestOptions? requestOptions = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        Guard.ArgumentIsNotNullOrEmpty(nameof(id), id);

        if (requestOptions is not null)
        {
            throw new NotImplementedException($"A non-null '{nameof(requestOptions)}' argument is not handled.");
        }

        var newDatabase = new InMemoryDatabase(this, id);
        var database = InMemoryDatabases.GetOrAdd(id, newDatabase);

        if (!ReferenceEquals(newDatabase, database))
        {
            return Task.FromResult<DatabaseResponse>(new InMemoryDatabaseResponse(database, HttpStatusCode.OK, 2));
        }

        return Task.FromResult<DatabaseResponse>(
            new InMemoryDatabaseResponse(database, HttpStatusCode.Created, 6.95));
    }

    /// <inheritdoc/>
    public override Task<DatabaseResponse> CreateDatabaseIfNotExistsAsync(string id,
        ThroughputProperties? throughputProperties, RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        Guard.ArgumentIsNotNullOrEmpty(nameof(id), id);

        if (requestOptions is not null)
        {
            throw new NotImplementedException($"A non-null '{nameof(requestOptions)}' argument is not handled.");
        }

        var newDatabase = new InMemoryDatabase(this, id);
        var database = InMemoryDatabases.GetOrAdd(id, newDatabase);

        if (!ReferenceEquals(newDatabase, database))
        {
            return Task.FromResult<DatabaseResponse>(new InMemoryDatabaseResponse(database, HttpStatusCode.OK, 2));
        }

        return Task.FromResult<DatabaseResponse>(
            new InMemoryDatabaseResponse(database, HttpStatusCode.Created, 6.95));
    }

    /// <inheritdoc/>
    public override Task<ResponseMessage> CreateDatabaseStreamAsync(DatabaseProperties databaseProperties,
        int? throughput = null, RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        return Task.FromException<ResponseMessage>(new NotImplementedException());
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        return ReferenceEquals(obj, this);
    }

    /// <inheritdoc/>
    public override Container GetContainer(string databaseId, string containerId)
    {
        return new InMemoryContainerProxy(this, new InMemoryDatabaseProxy(databaseId, this), containerId);
    }

    /// <inheritdoc/>
    public override Database GetDatabase(string id)
    {
        return new InMemoryDatabaseProxy(id, this);
    }

    /// <inheritdoc/>
    public override FeedIterator<T> GetDatabaseQueryIterator<T>(QueryDefinition queryDefinition,
        string? continuationToken = null, QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override FeedIterator<T> GetDatabaseQueryIterator<T>(string? queryText = null,
        string? continuationToken = null, QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override FeedIterator GetDatabaseQueryStreamIterator(QueryDefinition queryDefinition,
        string? continuationToken = null, QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override FeedIterator GetDatabaseQueryStreamIterator(string? queryText = null,
        string? continuationToken = null, QueryRequestOptions? requestOptions = null)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return RuntimeHelpers.GetHashCode(this);
    }

    /// <inheritdoc/>
    public override Task<AccountProperties> ReadAccountAsync()
    {
        return Task.FromException<AccountProperties>(new NotImplementedException());
    }

    /// <inheritdoc/>
    public override string? ToString()
    {
        return typeof(InMemoryCosmosClient).FullName;
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        // Intentionally not calling the base Dispose method. It throws a NullReferenceException due to the
        // ClientContext property being null.
    }
}
