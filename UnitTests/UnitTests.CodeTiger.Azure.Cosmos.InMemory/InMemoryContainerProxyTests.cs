using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CodeTiger.Azure.Cosmos.InMemory;
using Microsoft.Azure.Cosmos;
using Moq;
using Xunit;

namespace UnitTests.CodeTiger.Azure.Cosmos.InMemory;

public class InMemoryContainerProxyTests
{
    public class Constructor_InMemoryCosmosClient_InMemoryDatabaseProxy_String
    {
        [Fact]
        public void SetsDatabaseToProvidedDatabase()
        {
            var client = new InMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            Assert.Same(database, target.Database);
        }

        [Fact]
        public void SetsIdToProvidedId()
        {
            var client = new InMemoryCosmosClient();
            string id = Any.String();

            var target = new InMemoryContainerProxy(client, new InMemoryDatabaseProxy(Any.String(), client), id);

            Assert.Same(id, target.Id);
        }
    }

    public class CreateItemAsync_1_T_PartitionKey_ItemRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.CreateItemAsync(new object(), PartitionKey.Null, requestOptions, cancellationToken));
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenContainerDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.CreateItemAsync(new object(), PartitionKey.Null, requestOptions, cancellationToken));
        }

        [Fact]
        public async Task CallsRealContainerWhenContainerExists()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, client.MockContainerId);

            var item = new { Name = Any.String() };
            var partitionKey = new PartitionKey(item.Name);
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = await target.CreateItemAsync(item, partitionKey, requestOptions, cancellationToken);

            client.MockContainer.Verify(
                x => x.CreateItemAsync(item, partitionKey, requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class CreateItemStreamAsync_Stream_PartitionKey_ItemRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () =>
                {
                    using (var stream = new MemoryStream())
                    {
                        return target.CreateItemStreamAsync(stream, PartitionKey.Null, requestOptions,
                            cancellationToken);
                    }
                });
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenContainerDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () =>
                {
                    using (var stream = new MemoryStream())
                    {
                        return target.CreateItemStreamAsync(stream, PartitionKey.Null, requestOptions,
                            cancellationToken);
                    }
                });
        }

        [Fact]
        public async Task CallsRealContainerWhenContainerExists()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, client.MockContainerId);

            var item = new { Name = Any.String() };
            var partitionKey = new PartitionKey(item.Name);
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            var stream = new MemoryStream();
            using (stream)
            {
                _ = await target.CreateItemStreamAsync(stream, partitionKey, requestOptions, cancellationToken);
            }

            client.MockContainer.Verify(
                x => x.CreateItemStreamAsync(stream, partitionKey, requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class CreateTransactionalBatch_PartitionKey
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            Assert.Throws<NotImplementedException>(() => target.CreateTransactionalBatch(PartitionKey.Null));
        }
    }

    public class DeleteContainerAsync_ContainerRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ContainerRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.DeleteContainerAsync(requestOptions, cancellationToken));
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenContainerDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ContainerRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.DeleteContainerAsync(requestOptions, cancellationToken));
        }

        [Fact]
        public async Task CallsRealContainerWhenContainerExists()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, client.MockContainerId);

            var requestOptions = new ContainerRequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = await target.DeleteContainerAsync(requestOptions, cancellationToken);

            client.MockContainer
                .Verify(x => x.DeleteContainerAsync(requestOptions, cancellationToken), Times.Once());
        }
    }

    public class DeleteContainerStreamAsync_ContainerRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ContainerRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.DeleteContainerStreamAsync(requestOptions, cancellationToken));
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenContainerDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ContainerRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.DeleteContainerStreamAsync(requestOptions, cancellationToken));
        }

        [Fact]
        public async Task CallsRealContainerWhenContainerExists()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, client.MockContainerId);

            var requestOptions = new ContainerRequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = await target.DeleteContainerStreamAsync(requestOptions, cancellationToken);

            client.MockContainer
                .Verify(x => x.DeleteContainerStreamAsync(requestOptions, cancellationToken), Times.Once());
        }
    }

    public class DeleteItemAsync_1_String_PartitionKey_ItemRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.DeleteItemAsync<object>(Any.String(), PartitionKey.Null, requestOptions,
                    cancellationToken));
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenContainerDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.DeleteItemAsync<object>(Any.String(), PartitionKey.Null, requestOptions,
                    cancellationToken));
        }

        [Fact]
        public async Task CallsRealContainerWhenContainerExists()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, client.MockContainerId);

            string id = Any.String();
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = await target.DeleteItemAsync<object>(id, PartitionKey.Null, requestOptions, cancellationToken);

            client.MockContainer.Verify(
                x => x.DeleteItemAsync<object>(id, PartitionKey.Null, requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class DeleteItemStreamAsync_String_PartitionKey_ItemRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.DeleteItemStreamAsync(Any.String(), PartitionKey.Null, requestOptions,
                    cancellationToken));
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenContainerDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.DeleteItemStreamAsync(Any.String(), PartitionKey.Null, requestOptions,
                    cancellationToken));
        }

        [Fact]
        public async Task CallsRealContainerWhenContainerExists()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, client.MockContainerId);

            string id = Any.String();
            var partitionKey = new PartitionKey(Any.String());
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = await target.DeleteItemStreamAsync(id, partitionKey, requestOptions, cancellationToken);

            client.MockContainer.Verify(
                x => x.DeleteItemStreamAsync(id, partitionKey, requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class GetChangeFeedEstimator_String_Container
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            Assert.Throws<NotImplementedException>(() => target.GetChangeFeedEstimator(Any.String(), target));
        }
    }

    public class GetChangeFeedEstimatorBuilder_String_ChangeEstimationHandler_TimeSpan
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            Assert.Throws<NotImplementedException>(
                () => target.GetChangeFeedEstimatorBuilder(Any.String(), (_, _) => Task.CompletedTask, null));
        }
    }

    public class GetChangeFeedIterator_1_ChangeFeedStartFrom_ChangeFeedMode_ChangeFeedRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            Assert.Throws<NotImplementedException>(
                () => target.GetChangeFeedIterator<object>(ChangeFeedStartFrom.Beginning(),
                    ChangeFeedMode.LatestVersion, null));
        }
    }

    public class GetChangeFeedProcessorBuilder_1_String_ChangesHandler1
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);
            Container.ChangesHandler<object> onChangesDelegate = (_, _) => Task.CompletedTask;

            var target = new InMemoryContainerProxy(client, database, Any.String());

            Assert.Throws<NotImplementedException>(
                () => target.GetChangeFeedProcessorBuilder<object>(Any.String(), onChangesDelegate));
        }
    }

    public class GetChangeFeedProcessorBuilder_1_String_ChangeFeedHandler1
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);
            Container.ChangeFeedHandler<object> onChangesDelegate = (_, _, _) => Task.CompletedTask;

            var target = new InMemoryContainerProxy(client, database, Any.String());

            Assert.Throws<NotImplementedException>(
                () => target.GetChangeFeedProcessorBuilder<object>(Any.String(), onChangesDelegate));
        }
    }

    public class GetChangeFeedProcessorBuilder_String_ChangeFeedStreamHandler
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);
            Container.ChangeFeedStreamHandler onChangesDelegate = (_, _, _) => Task.CompletedTask;

            var target = new InMemoryContainerProxy(client, database, Any.String());

            Assert.Throws<NotImplementedException>(
                () => target.GetChangeFeedProcessorBuilder(Any.String(), onChangesDelegate));
        }
    }

    public class GetChangeFeedProcessorBuilderWithManualCheckpoint_1_String_ChangeFeedHandlerWithManualCheckpoint1
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);
            Container.ChangeFeedHandlerWithManualCheckpoint<object> onChangesDelegate
                = (_, _, _, _) => Task.CompletedTask;

            var target = new InMemoryContainerProxy(client, database, Any.String());

            Assert.Throws<NotImplementedException>(
                () => target.GetChangeFeedProcessorBuilderWithManualCheckpoint(Any.String(), onChangesDelegate));
        }
    }

    [SuppressMessage("CodeTiger.Layout", "CT3531:Lines should not exceed the maximum length of 115",
        Justification = "Allowed, to be able to contain the entire method signature.")]
    public class GetChangeFeedProcessorBuilderWithManualCheckpoint_String_ChangeFeedStreamHandlerWithManualCheckpoint
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);
            Container.ChangeFeedStreamHandlerWithManualCheckpoint onChangesDelegate
                = (_, _, _, _) => Task.CompletedTask;

            var target = new InMemoryContainerProxy(client, database, Any.String());

            Assert.Throws<NotImplementedException>(
                () => target.GetChangeFeedProcessorBuilderWithManualCheckpoint(Any.String(), onChangesDelegate));
        }
    }

    public class GetChangeFeedStreamIterator_ChangeFeedStartFrom_ChangeFeedMode_ChangeFeedRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            Assert.Throws<NotImplementedException>(
                () => target.GetChangeFeedStreamIterator(ChangeFeedStartFrom.Now(), ChangeFeedMode.LatestVersion,
                    new ChangeFeedRequestOptions()));
        }
    }

    public class GetFeedRangesAsync_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.GetFeedRangesAsync(cancellationToken));
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenContainerDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.GetFeedRangesAsync(cancellationToken));
        }

        [Fact]
        public async Task CallsRealContainerWhenContainerExists()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, client.MockContainerId);

            var cancellationToken = new CancellationToken(false);

            _ = await target.GetFeedRangesAsync(cancellationToken);

            client.MockContainer.Verify(x => x.GetFeedRangesAsync(cancellationToken), Times.Once());
        }
    }

    public class GetItemLinqQueryable_1_Boolean_String_QueryRequestOptions_CosmosLinqSerializerOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            Assert.Throws<NotImplementedException>(
                () => target.GetItemLinqQueryable<object>(Any.Boolean(), Any.String(), new QueryRequestOptions(),
                    new CosmosLinqSerializerOptions()));
        }
    }

    public class GetItemQueryIterator_1_QueryDefinition_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);
            var queryDefinition = new QueryDefinition("SELECT * FROM c");

            var target = new InMemoryContainerProxy(client, database, Any.String());
            
            Assert.Throws<NotImplementedException>(
                () => target.GetItemQueryIterator<object>(queryDefinition, Any.String(),
                    new QueryRequestOptions()));
        }
    }

    public class GetItemQueryIterator_1_String_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);
            string query = "SELECT * FROM c";

            var target = new InMemoryContainerProxy(client, database, Any.String());

            Assert.Throws<NotImplementedException>(
                () => target.GetItemQueryIterator<object>(query, Any.String(), new QueryRequestOptions()));
        }
    }

    public class GetItemQueryIterator_1_FeedRange_QueryDefinition_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);
            var queryDefinition = new QueryDefinition("SELECT * FROM c");

            var target = new InMemoryContainerProxy(client, database, Any.String());

            Assert.Throws<NotImplementedException>(
                () => target.GetItemQueryIterator<object>(FeedRange.FromPartitionKey(PartitionKey.Null),
                    queryDefinition, Any.String(), new QueryRequestOptions()));
        }
    }

    public class GetItemQueryStreamIterator_QueryDefinition_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);
            var queryDefinition = new QueryDefinition("SELECT * FROM c");

            var target = new InMemoryContainerProxy(client, database, Any.String());

            Assert.Throws<NotImplementedException>(
                () => target.GetItemQueryStreamIterator(queryDefinition, Any.String(), new QueryRequestOptions()));
        }
    }

    public class GetItemQueryStreamIterator_String_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);
            string query = "SELECT * FROM c";

            var target = new InMemoryContainerProxy(client, database, Any.String());

            Assert.Throws<NotImplementedException>(
                () => target.GetItemQueryStreamIterator(query, Any.String(), new QueryRequestOptions()));
        }
    }

    public class GetItemQueryStreamIterator_FeedRange_QueryDefinition_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);
            var queryDefinition = new QueryDefinition("SELECT * FROM c");

            var target = new InMemoryContainerProxy(client, database, Any.String());

            Assert.Throws<NotImplementedException>(
                () => target.GetItemQueryStreamIterator(FeedRange.FromPartitionKey(PartitionKey.Null),
                    queryDefinition, Any.String(), new QueryRequestOptions()));
        }
    }

    public class PatchItemAsync_1_String_PartitionKey_IReadOnlyList_PatchItemRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var patchOperations = new List<PatchOperation>();
            var requestOptions = new PatchItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.PatchItemAsync<object>(Any.String(), PartitionKey.Null, patchOperations,
                    requestOptions, cancellationToken));
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenContainerDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var patchOperations = new List<PatchOperation>();
            var requestOptions = new PatchItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.PatchItemAsync<object>(Any.String(), PartitionKey.Null, patchOperations,
                    requestOptions, cancellationToken));
        }

        [Fact]
        public async Task CallsRealContainerWhenContainerExists()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, client.MockContainerId);

            string id = Any.String();
            var patchOperations = new List<PatchOperation>();
            var requestOptions = new PatchItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = await target.PatchItemAsync<object>(id, PartitionKey.Null, patchOperations,
                requestOptions, cancellationToken);

            client.MockContainer.Verify(
                x => x.PatchItemAsync<object>(id, PartitionKey.Null, patchOperations, requestOptions,
                    cancellationToken),
                Times.Once());
        }
    }

    public class PatchItemStreamAsync_String_PartitionKey_IReadOnlyList_PatchItemRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var patchOperations = new List<PatchOperation>();
            var requestOptions = new PatchItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.PatchItemStreamAsync(Any.String(), PartitionKey.Null, patchOperations,
                    requestOptions, cancellationToken));
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenContainerDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var patchOperations = new List<PatchOperation>();
            var requestOptions = new PatchItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.PatchItemStreamAsync(Any.String(), PartitionKey.Null, patchOperations,
                    requestOptions, cancellationToken));
        }

        [Fact]
        public async Task CallsRealContainerWhenContainerExists()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, client.MockContainerId);

            string id = Any.String();
            var patchOperations = new List<PatchOperation>();
            var requestOptions = new PatchItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = await target.PatchItemStreamAsync(id, PartitionKey.Null, patchOperations,
                requestOptions, cancellationToken);

            client.MockContainer.Verify(
                x => x.PatchItemStreamAsync(id, PartitionKey.Null, patchOperations, requestOptions,
                    cancellationToken),
                Times.Once());
        }
    }

    public class ReadContainerAsync_ContainerRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ContainerRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReadContainerAsync(requestOptions, cancellationToken));
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenContainerDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ContainerRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReadContainerAsync(requestOptions, cancellationToken));
        }

        [Fact]
        public async Task CallsRealContainerWhenContainerExists()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, client.MockContainerId);

            var requestOptions = new ContainerRequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = await target.ReadContainerAsync(requestOptions, cancellationToken);

            client.MockContainer.Verify(
                x => x.ReadContainerAsync(requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class ReadContainerStreamAsync_ContainerRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ContainerRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReadContainerStreamAsync(requestOptions, cancellationToken));
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenContainerDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ContainerRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReadContainerStreamAsync(requestOptions, cancellationToken));
        }

        [Fact]
        public async Task CallsRealContainerWhenContainerExists()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, client.MockContainerId);

            var requestOptions = new ContainerRequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = await target.ReadContainerStreamAsync(requestOptions, cancellationToken);

            client.MockContainer.Verify(
                x => x.ReadContainerStreamAsync(requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class ReadItemAsync_1_String_PartitionKey_ItemRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReadItemAsync<object>(Any.String(), PartitionKey.Null, requestOptions,
                    cancellationToken));
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenContainerDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReadItemAsync<object>(Any.String(), PartitionKey.Null, requestOptions,
                    cancellationToken));
        }

        [Fact]
        public async Task CallsRealContainerWhenContainerExists()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, client.MockContainerId);

            string id = Any.String();
            var partitionKey = new PartitionKey(Any.String());
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = await target.ReadItemAsync<object>(id, partitionKey, requestOptions, cancellationToken);

            client.MockContainer.Verify(
                x => x.ReadItemAsync<object>(id, partitionKey, requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class ReadItemStreamAsync_String_PartitionKey_ItemRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReadItemStreamAsync(Any.String(), PartitionKey.Null, requestOptions,
                    cancellationToken));
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenContainerDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReadItemStreamAsync(Any.String(), PartitionKey.Null, requestOptions,
                    cancellationToken));
        }

        [Fact]
        public async Task CallsRealContainerWhenContainerExists()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, client.MockContainerId);

            string id = Any.String();
            var partitionKey = new PartitionKey(Any.String());
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = await target.ReadItemStreamAsync(id, partitionKey, requestOptions, cancellationToken);

            client.MockContainer.Verify(
                x => x.ReadItemStreamAsync(id, partitionKey, requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class ReadManyItemsAsync_1_IReadOnlyList_ReadManyRequestOptions_CancellationToken
    {
    }

    public class ReadManyItemsStreamAsync_IReadOnlyList_ReadManyRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var items = new List<(string, PartitionKey)>();
            var requestOptions = new ReadManyRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReadManyItemsStreamAsync(items, requestOptions, cancellationToken));
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenContainerDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var items = new List<(string, PartitionKey)>();
            var requestOptions = new ReadManyRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReadManyItemsStreamAsync(items, requestOptions, cancellationToken));
        }

        [Fact]
        public async Task CallsRealContainerWhenContainerExists()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, client.MockContainerId);

            var items = new List<(string, PartitionKey)>();
            var requestOptions = new ReadManyRequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = await target.ReadManyItemsStreamAsync(items, requestOptions, cancellationToken);

            client.MockContainer.Verify(
                x => x.ReadManyItemsStreamAsync(items, requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class ReadThroughputAsync_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReadThroughputAsync(cancellationToken));
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenContainerDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReadThroughputAsync(cancellationToken));
        }

        [Fact]
        public async Task CallsRealContainerWhenContainerExists()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, client.MockContainerId);

            var cancellationToken = new CancellationToken(false);

            _ = await target.ReadThroughputAsync(cancellationToken);

            client.MockContainer.Verify(
                x => x.ReadThroughputAsync(cancellationToken),
                Times.Once());
        }
    }

    public class ReadThroughputAsync_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ReadManyRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReadThroughputAsync(requestOptions, cancellationToken));
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenContainerDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ReadManyRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReadThroughputAsync(requestOptions, cancellationToken));
        }

        [Fact]
        public async Task CallsRealContainerWhenContainerExists()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, client.MockContainerId);

            var requestOptions = new ReadManyRequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = await target.ReadThroughputAsync(requestOptions, cancellationToken);

            client.MockContainer.Verify(
                x => x.ReadThroughputAsync(requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class ReplaceContainerAsync_ContainerProperties_ContainerRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var containerProperties = new ContainerProperties();
            var requestOptions = new ContainerRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReplaceContainerAsync(containerProperties, requestOptions, cancellationToken));
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenContainerDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var containerProperties = new ContainerProperties();
            var requestOptions = new ContainerRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReplaceContainerAsync(containerProperties, requestOptions, cancellationToken));
        }

        [Fact]
        public async Task CallsRealContainerWhenContainerExists()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, client.MockContainerId);

            var containerProperties = new ContainerProperties();
            var requestOptions = new ContainerRequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = await target.ReplaceContainerAsync(containerProperties, requestOptions, cancellationToken);

            client.MockContainer.Verify(
                x => x.ReplaceContainerAsync(containerProperties, requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class ReplaceContainerStreamAsync_ContainerProperties_ContainerRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var containerProperties = new ContainerProperties();
            var requestOptions = new ContainerRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReplaceContainerStreamAsync(containerProperties, requestOptions, cancellationToken));
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenContainerDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var containerProperties = new ContainerProperties();
            var requestOptions = new ContainerRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReplaceContainerStreamAsync(containerProperties, requestOptions, cancellationToken));
        }

        [Fact]
        public async Task CallsRealContainerWhenContainerExists()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, client.MockContainerId);

            var containerProperties = new ContainerProperties();
            var requestOptions = new ContainerRequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = await target.ReplaceContainerStreamAsync(containerProperties, requestOptions, cancellationToken);

            client.MockContainer.Verify(
                x => x.ReplaceContainerStreamAsync(containerProperties, requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class ReplaceItemAsync_1_T_String_PartitionKey_ItemRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReplaceItemAsync(new { }, Any.String(), PartitionKey.Null, requestOptions,
                    cancellationToken));
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenContainerDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReplaceItemAsync(new { }, Any.String(), PartitionKey.Null, requestOptions,
                    cancellationToken));
        }

        [Fact]
        public async Task CallsRealContainerWhenContainerExists()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, client.MockContainerId);

            var item = new { Name = Any.String() };
            string id = item.Name;
            var partitionKey = new PartitionKey(item.Name);
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = await target.ReplaceItemAsync(item, id, partitionKey, requestOptions, cancellationToken);

            client.MockContainer.Verify(
                x => x.ReplaceItemAsync(item, id, partitionKey, requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class ReplaceItemStreamAsync_Stream_String_PartitionKey_ItemRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var partitionKey = new PartitionKey(Any.String());
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () =>
                {
                    using (var stream = new MemoryStream())
                    {
                        return target.ReplaceItemStreamAsync(stream, Any.String(), partitionKey, requestOptions,
                            cancellationToken);
                    }
                });
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenContainerDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var partitionKey = new PartitionKey(Any.String());
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () =>
                {
                    using (var stream = new MemoryStream())
                    {
                        return target.ReplaceItemStreamAsync(stream, Any.String(), partitionKey, requestOptions,
                            cancellationToken);
                    }
                });
        }

        [Fact]
        public async Task CallsRealContainerWhenContainerExists()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, client.MockContainerId);

            string id = Any.String();
            var partitionKey = new PartitionKey(id);
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            var stream = new MemoryStream();
            using (stream)
            {
                _ = await target.ReplaceItemStreamAsync(stream, id, partitionKey, requestOptions,
                    cancellationToken);
            }

            client.MockContainer.Verify(
                x => x.ReplaceItemStreamAsync(stream, id, partitionKey, requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class ReplaceThroughputAsync_Int32_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReplaceThroughputAsync(throughput, requestOptions, cancellationToken));
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenContainerDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReplaceThroughputAsync(throughput, requestOptions, cancellationToken));
        }

        [Fact]
        public async Task CallsRealContainerWhenContainerExists()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, client.MockContainerId);

            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = await target.ReplaceThroughputAsync(throughput, requestOptions, cancellationToken);

            client.MockContainer.Verify(
                x => x.ReplaceThroughputAsync(throughput, requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class ReplaceThroughputAsync_ThroughputProperties_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var throughputProperties = ThroughputProperties.CreateManualThroughput(Any.Int32());
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReplaceThroughputAsync(throughputProperties, requestOptions, cancellationToken));
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenContainerDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var throughputProperties = ThroughputProperties.CreateManualThroughput(Any.Int32());
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReplaceThroughputAsync(throughputProperties, requestOptions, cancellationToken));
        }

        [Fact]
        public async Task CallsRealContainerWhenContainerExists()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, client.MockContainerId);

            var throughputProperties = ThroughputProperties.CreateManualThroughput(Any.Int32());
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = await target.ReplaceThroughputAsync(throughputProperties, requestOptions, cancellationToken);

            client.MockContainer.Verify(
                x => x.ReplaceThroughputAsync(throughputProperties, requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class UpsertItemAsync_1_T_PartitionKey_ItemRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.UpsertItemAsync(new { }, PartitionKey.Null, requestOptions,
                    cancellationToken));
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenContainerDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.UpsertItemAsync(new { }, PartitionKey.Null, requestOptions,
                    cancellationToken));
        }

        [Fact]
        public async Task CallsRealContainerWhenContainerExists()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, client.MockContainerId);

            var item = new { Name = Any.String() };
            var partitionKey = new PartitionKey(item.Name);
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = await target.UpsertItemAsync(item, partitionKey, requestOptions, cancellationToken);

            client.MockContainer.Verify(
                x => x.UpsertItemAsync(item, partitionKey, requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class UpsertItemStreamAsync_Stream_PartitionKey_ItemRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(Any.String(), client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var partitionKey = new PartitionKey(Any.String());
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () =>
                {
                    using (var stream = new MemoryStream())
                    {
                        return target.UpsertItemStreamAsync(stream, partitionKey, requestOptions,
                            cancellationToken);
                    }
                });
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenContainerDoesNotExist()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, Any.String());

            var partitionKey = new PartitionKey(Any.String());
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () =>
                {
                    using (var stream = new MemoryStream())
                    {
                        return target.UpsertItemStreamAsync(stream, partitionKey, requestOptions,
                            cancellationToken);
                    }
                });
        }

        [Fact]
        public async Task CallsRealContainerWhenContainerExists()
        {
            var client = new TestingInMemoryCosmosClient();
            var database = new InMemoryDatabaseProxy(client.MockDatabaseId, client);

            var target = new InMemoryContainerProxy(client, database, client.MockContainerId);

            var partitionKey = new PartitionKey(Any.String());
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = new CancellationToken(false);

            var stream = new MemoryStream();
            using (stream)
            {
                _ = await target.UpsertItemStreamAsync(stream, partitionKey, requestOptions,
                    cancellationToken);
            }

            client.MockContainer.Verify(
                x => x.UpsertItemStreamAsync(stream, partitionKey, requestOptions, cancellationToken),
                Times.Once());
        }
    }

    internal class TestingInMemoryCosmosClient : InMemoryCosmosClient
    {
        public string MockDatabaseId { get; }

        public string MockContainerId { get; }

        public Mock<InMemoryDatabase> MockDatabase { get; }

        public Mock<InMemoryContainer> MockContainer { get; }

        public TestingInMemoryCosmosClient()
        {
            MockDatabaseId = Any.String();
            MockDatabase = new Mock<InMemoryDatabase>(this, MockDatabaseId);
            InMemoryDatabases.TryAdd(MockDatabaseId, MockDatabase.Object);
            
            MockContainerId = Any.String();
            MockContainer = new Mock<InMemoryContainer>(Any.Int32(), MockDatabase.Object,
                new ContainerProperties { Id = MockContainerId });
            MockDatabase.Object.InMemoryContainers.TryAdd(MockContainerId, MockContainer.Object);
        }

        internal class TestingInMemoryDatabase : InMemoryDatabase
        {
            public TestingInMemoryDatabase(InMemoryCosmosClient testingCosmosClient, string id)
                : base(testingCosmosClient, id)
            {
            }
        }
    }
}
