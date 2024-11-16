using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodeTiger.Azure.Cosmos.InMemory;
using Microsoft.Azure.Cosmos;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace UnitTests.CodeTiger.Azure.Cosmos.InMemory;

public class InMemoryContainerTests
{
    public class Constructor_CosmosSerializer_InMemoryDatabase_ContainerProperties_ByteArray
    {
        [Fact]
        public void SetsDatabaseToProvidedDatabase()
        {
            var client = new InMemoryCosmosClient(
                new CosmosClientOptions { Serializer = new InMemoryCosmosJsonDotNetSerializer() });
            var database = new InMemoryDatabase(client, Any.String());

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            Assert.Same(database, target.Database);
        }

        [Fact]
        public void SetsTestingDatabaseToProvidedDatabase()
        {
            var client = new InMemoryCosmosClient(
                new CosmosClientOptions { Serializer = new InMemoryCosmosJsonDotNetSerializer() });
            var database = new InMemoryDatabase(client, Any.String());

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            Assert.Same(database, target.InMemoryDatabase);
        }

        [Fact]
        public void SetsSerializerToClientSerializer()
        {
            var client = new InMemoryCosmosClient(
                new CosmosClientOptions { Serializer = new InMemoryCosmosJsonDotNetSerializer() });
            var database = new InMemoryDatabase(client, Any.String());

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            Assert.Same(client.Serializer, target.Serializer);
        }

        [Fact]
        public void SetsIdFromProvidedContainerProperties()
        {
            var client = new InMemoryCosmosClient(
                new CosmosClientOptions { Serializer = new InMemoryCosmosJsonDotNetSerializer() });
            var database = new InMemoryDatabase(client, Any.String());
            var containerProperties = new ContainerProperties { Id = Any.String() };

            var target = new InMemoryContainer(Any.Int32(), database, containerProperties);

            Assert.Same(containerProperties.Id, target.Id);
        }

        [Fact]
        public void SetsRidToProvidedValue()
        {
            var client = new InMemoryCosmosClient(
                new CosmosClientOptions { Serializer = new InMemoryCosmosJsonDotNetSerializer() });
            var database = new InMemoryDatabase(client, Any.String());
            var containerProperties = new ContainerProperties { Id = Any.String() };
            int rid = Any.Int32();

            var target = new InMemoryContainer(rid, database, containerProperties);

            Assert.Equal(rid, target.Rid);
        }

        [Fact]
        public void LeavesPartitionsEmpty()
        {
            var client = new InMemoryCosmosClient(
                new CosmosClientOptions { Serializer = new InMemoryCosmosJsonDotNetSerializer() });
            var database = new InMemoryDatabase(client, Any.String());
            var containerProperties = new ContainerProperties { Id = Any.String() };

            var target = new InMemoryContainer(Any.Int32(), database, containerProperties);

            Assert.Empty(target.Partitions);
        }
    }

    public class CreateItemAsync_1_T_PartitionKey_ItemRequestOptions_CancellationToken
    {
        [Fact]
        public async Task AddsNewItemToSpecifiedPartition()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var partitionKey = new PartitionKey(Any.String());
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = CancellationToken.None;

            var item = new Thing { Id = Guid.NewGuid(), Name = Any.String() };

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            _ = await target.CreateItemAsync(item, partitionKey, requestOptions, cancellationToken);

            Assert.Collection(target.Partitions,
                x =>
                {
                    Assert.Equal(partitionKey, x.Key);
                    Assert.Collection(x.Value, y => Assert.Equal(item.Id.ToString(), y.Key));
                });
        }

        [Fact]
        public async Task AddsNewItemToConfiguredPartitionWhenNoPartitionKeyIsProvided()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());
            
            var id = Guid.NewGuid();
            var partitionKey = new PartitionKey(id.ToString());
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = CancellationToken.None;

            var item = new Thing { Id = id, Name = Any.String() };

            var target = new InMemoryContainer(Any.Int32(), database,
                new ContainerProperties(Any.String(), "/id"));

            _ = await target.CreateItemAsync(item, null, requestOptions, cancellationToken);

            Assert.Collection(target.Partitions,
                x =>
                {
                    Assert.Equal(partitionKey, x.Key);
                    Assert.Collection(x.Value, y => Assert.Equal(item.Id.ToString(), y.Key));
                });
        }

        [Fact]
        public async Task SetsRidPropertyOnNewItem()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var partitionKey = new PartitionKey(Any.String());
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = CancellationToken.None;

            var item = new Thing { Id = Guid.NewGuid(), Name = Any.String() };

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            _ = await target.CreateItemAsync(item, partitionKey, requestOptions, cancellationToken);

            var itemJObject = target.Partitions[partitionKey][item.Id.ToString()];

            Assert.Matches("^[a-zA-Z0-9+/]{22}==$", itemJObject["_rid"]?.ToString());
        }

        [Fact]
        public async Task SetsSelfPropertyOnNewItem()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var partitionKey = new PartitionKey(Any.String());
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = CancellationToken.None;

            var item = new Thing { Id = Guid.NewGuid(), Name = Any.String() };

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            _ = await target.CreateItemAsync(item, partitionKey, requestOptions, cancellationToken);

            var itemJObject = target.Partitions[partitionKey][item.Id.ToString()];

            Assert.Matches("^dbs/.+/colls/.+/docs/.+$", itemJObject["_self"]?.ToString());
        }

        [Fact]
        public async Task SetsETagPropertyOnNewItem()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var partitionKey = new PartitionKey(Any.String());
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = CancellationToken.None;

            var item = new Thing { Id = Guid.NewGuid(), Name = Any.String() };

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            _ = await target.CreateItemAsync(item, partitionKey, requestOptions, cancellationToken);

            var itemJObject = target.Partitions[partitionKey][item.Id.ToString()];

            Assert.Matches("^\"[0-9a-z]{8}-[0-9a-z]{4}-[0-9a-z]{4}-[0-9a-z]{4}-[0-9a-z]{12}\"$",
                itemJObject["_etag"]?.ToString());
        }

        [Fact]
        public async Task SetsAttachmentsPropertyOnNewItem()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var partitionKey = new PartitionKey(Any.String());
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = CancellationToken.None;

            var item = new Thing { Id = Guid.NewGuid(), Name = Any.String() };

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            _ = await target.CreateItemAsync(item, partitionKey, requestOptions, cancellationToken);

            var itemJObject = target.Partitions[partitionKey][item.Id.ToString()];

            Assert.Equal("attachments/", itemJObject["_attachments"]?.ToString());
        }

        [Fact]
        public async Task SetsTimestampPropertyOnNewItem()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var partitionKey = new PartitionKey(Any.String());
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = CancellationToken.None;

            var item = new Thing { Id = Guid.NewGuid(), Name = Any.String() };

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            _ = await target.CreateItemAsync(item, partitionKey, requestOptions, cancellationToken);

            var itemJObject = target.Partitions[partitionKey][item.Id.ToString()];

            Assert.Equal(JTokenType.Integer, itemJObject["_ts"]?.Type);
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenItemAlreadyExists()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var partitionKey = new PartitionKey(Any.String());
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = CancellationToken.None;

            var duplicateItem = new Thing { Id = Guid.NewGuid(), Name = Any.String() };

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            var partition = new ConcurrentDictionary<string, JObject>();
            target.Partitions.TryAdd(partitionKey, partition);
            partition.TryAdd(duplicateItem.Id.ToString(), new JObject());

            await Assert.ThrowsAsync<CosmosException>(
                () => target.CreateItemAsync(duplicateItem, partitionKey, requestOptions, cancellationToken));
        }

        [Fact]
        public async Task SerializesWithClientSerializer()
        {
            var mockSerializer = new Mock<CosmosSerializer>();
            mockSerializer.Setup(x => x.ToStream(It.IsAny<Thing>()))
                .Returns(new MemoryStream(Encoding.UTF8.GetBytes("{}")));
            
            var testingCosmosClient = new TestingInMemoryCosmosClient(
                new CosmosClientOptions { Serializer = mockSerializer.Object });
            var database = new InMemoryDatabase(testingCosmosClient, Any.String());

            var partitionKey = new PartitionKey(Any.String());
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = CancellationToken.None;

            var item = new Thing { Id = Guid.NewGuid(), Name = Any.String() };

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            _ = await target.CreateItemAsync(item, partitionKey, requestOptions, cancellationToken);

            mockSerializer.Verify(x => x.ToStream(It.IsAny<Thing>()), Times.Once());
        }
    }

    public class CreateItemStreamAsync_Stream_PartitionKey_ItemRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var partitionKey = PartitionKey.Null;
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = CancellationToken.None;

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            await Assert.ThrowsAsync<NotImplementedException>(
                () =>
                {
                    using (var stream = new MemoryStream())
                    {
                        return target.CreateItemStreamAsync(stream, partitionKey, requestOptions,
                            cancellationToken);
                    }
                });
        }
    }

    public class CreateTransactionalBatch_PartitionKey
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var partitionKey = new PartitionKey(Any.String());

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            Assert.Throws<NotImplementedException>(
                () => target.CreateTransactionalBatch(partitionKey));
        }
    }

    public class DeleteContainerAsync_ContainerRequestOptions_CancellationToken
    {
        [Fact]
        public async Task DeletesExistingContainer()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            string id = Any.String();
            var properties = new ContainerProperties(id, "/partitionKey");
            database.InMemoryContainers.TryAdd(id, new InMemoryContainer(Any.Int32(), database, properties));
            var requestOptions = new ContainerRequestOptions();
            var cancellationToken = CancellationToken.None;

            var target = new InMemoryContainer(Any.Int32(), database, properties);

            _ = await target.DeleteContainerAsync(requestOptions, cancellationToken);

            Assert.Empty(database.InMemoryContainers);
        }

        [Fact]
        public async Task ReturnsExistingContainer()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            string id = Any.String();
            var properties = new ContainerProperties(id, "/partitionKey");
            database.InMemoryContainers.TryAdd(id, new InMemoryContainer(Any.Int32(), database, properties));
            var requestOptions = new ContainerRequestOptions();
            var cancellationToken = CancellationToken.None;

            var target = new InMemoryContainer(Any.Int32(), database, properties);

            var actual = await target.DeleteContainerAsync(requestOptions, cancellationToken);

            Assert.Equal(id, actual.Resource.Id);
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenContainerDoesNotExist()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var properties = new ContainerProperties(Any.String(), "/partitionKey");
            var requestOptions = new ContainerRequestOptions();
            var cancellationToken = CancellationToken.None;

            var target = new InMemoryContainer(Any.Int32(), database, properties);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.DeleteContainerAsync(requestOptions, cancellationToken));
        }
    }

    public class DeleteContainerStreamAsync_ContainerRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var requestOptions = new ContainerRequestOptions();
            var cancellationToken = CancellationToken.None;

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            await Assert.ThrowsAsync<NotImplementedException>(
                () => target.DeleteContainerStreamAsync(requestOptions, cancellationToken));
        }
    }

    public class DeleteItemAsync_1_String_PartitionKey_ItemRequestOptions_CancellationToken
    {
        [Fact]
        public async Task DeletesExistingItem()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var id = Guid.NewGuid();
            var partitionKey = new PartitionKey(id.ToString());
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = CancellationToken.None;

            var item = new Thing { Id = id, Name = Any.String() };

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            var partition = new ConcurrentDictionary<string, JObject>();
            target.Partitions.TryAdd(partitionKey, partition);
            partition.TryAdd(item.Id.ToString(), new JObject());

            _ = await target
                .DeleteItemAsync<Thing>(id.ToString(), partitionKey, requestOptions, cancellationToken);

            Assert.Empty(partition);
        }

        [Fact]
        public async Task ReturnsExistingItem()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var id = Guid.NewGuid();
            var partitionKey = new PartitionKey(id.ToString());
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = CancellationToken.None;

            var item = new Thing { Id = id, Name = Any.String() };

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            var partition = new ConcurrentDictionary<string, JObject>();
            target.Partitions.TryAdd(partitionKey, partition);
            
            using (var itemJsonStream = target.Serializer.ToStream(item))
            using (var itemJsonStreamReader = new StreamReader(itemJsonStream))
            {
                partition.TryAdd(item.Id.ToString(), JObject.Parse(itemJsonStreamReader.ReadToEnd()));
            }

            var actual = await target
                .DeleteItemAsync<Thing>(id.ToString(), partitionKey, requestOptions, cancellationToken);

            Assert.Equal(id, actual.Resource.Id);
        }

        [Fact]
        public async Task ThrowsSomethingWhenItemDoesNotExist()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            string id = Any.String();
            var partitionKey = new PartitionKey(id);
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = CancellationToken.None;

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            await Assert.ThrowsAsync<CosmosException>(
                () => target.DeleteItemAsync<Thing>(id, partitionKey, requestOptions, cancellationToken));
        }
    }

    public class DeleteItemStreamAsync_String_PartitionKey_ItemRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var partitionKey = new PartitionKey(Any.String());
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = CancellationToken.None;

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            await Assert.ThrowsAsync<NotImplementedException>(
                () => target.DeleteItemStreamAsync(Any.String(), partitionKey, requestOptions, cancellationToken));
        }
    }

    public class GetChangeFeedEstimator_String_Container
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            Assert.Throws<NotImplementedException>(
                () => target.GetChangeFeedEstimator(Any.String(), target));
        }
    }

    public class GetChangeFeedEstimatorBuilder_String_ChangeEstimationHandler_TimeSpan
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            Container.ChangesEstimationHandler estimationDelegate = (_, _) => Task.CompletedTask;

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            Assert.Throws<NotImplementedException>(
                () => target.GetChangeFeedEstimatorBuilder(Any.String(), estimationDelegate,
                    TimeSpan.FromTicks(Any.Int64())));
        }
    }

    public class GetChangeFeedIterator_1_ChangeFeedStartFrom_ChangeFeedMode_ChangeFeedRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var startFrom = ChangeFeedStartFrom.Now();
            var changeFeedMode = ChangeFeedMode.LatestVersion;
            var requestOptions = new ChangeFeedRequestOptions();

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            Assert.Throws<NotImplementedException>(
                () => target.GetChangeFeedIterator<object>(startFrom, changeFeedMode, requestOptions));
        }
    }

    public class GetChangeFeedProcessorBuilder_1_String_ChangesHandler1
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            Container.ChangesHandler<object> onChangesDelegate = (_, _) => Task.CompletedTask;

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            Assert.Throws<NotImplementedException>(
                () => target.GetChangeFeedProcessorBuilder<object>(Any.String(), onChangesDelegate));
        }
    }

    public class GetChangeFeedProcessorBuilder_1_String_ChangeFeedHandler1
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            Container.ChangeFeedHandler<object> onChangesDelegate = (_, _, _) => Task.CompletedTask;

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            Assert.Throws<NotImplementedException>(
                () => target.GetChangeFeedProcessorBuilder<object>(Any.String(), onChangesDelegate));
        }
    }

    public class GetChangeFeedProcessorBuilder_String_ChangeFeedStreamHandler
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            Container.ChangeFeedStreamHandler onChangesDelegate = (_, _, _) => Task.CompletedTask;

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            Assert.Throws<NotImplementedException>(
                () => target.GetChangeFeedProcessorBuilder(Any.String(), onChangesDelegate));
        }
    }

    public class GetChangeFeedProcessorBuilderWithManualCheckpoint_1_String_ChangeFeedHandlerWithManualCheckpoint1
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            Container.ChangeFeedHandlerWithManualCheckpoint<object> onChangesDelegate
                = (_, _, _, _) => Task.CompletedTask;

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            Assert.Throws<NotImplementedException>(
                () => target.GetChangeFeedProcessorBuilderWithManualCheckpoint<object>(Any.String(),
                    onChangesDelegate));
        }
    }

    [SuppressMessage("CodeTiger.Layout", "CT3531:Lines should not exceed the maximum length of 115",
        Justification = "Allowed, to be able to contain the entire method signature.")]
    public class GetChangeFeedProcessorBuilderWithManualCheckpoint_String_ChangeFeedStreamHandlerWithManualCheckpoint
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            Container.ChangeFeedStreamHandlerWithManualCheckpoint onChangesDelegate
                = (_, _, _, _) => Task.CompletedTask;

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            Assert.Throws<NotImplementedException>(
                () => target.GetChangeFeedProcessorBuilderWithManualCheckpoint(Any.String(),
                    onChangesDelegate));
        }
    }

    public class GetChangeFeedStreamIterator_ChangeFeedStartFrom_ChangeFeedMode_ChangeFeedRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var startFrom = ChangeFeedStartFrom.Now();
            var changeFeedMode = ChangeFeedMode.LatestVersion;
            var requestOptions = new ChangeFeedRequestOptions();

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            Assert.Throws<NotImplementedException>(
                () => target.GetChangeFeedStreamIterator(startFrom, changeFeedMode, requestOptions));
        }
    }

    public class GetFeedRangesAsync_CancellationToken
    {
        [Fact]
        public async Task ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var cancellationToken = CancellationToken.None;

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            await Assert.ThrowsAsync<NotImplementedException>(() => target.GetFeedRangesAsync(cancellationToken));
        }
    }

    public class GetItemLinqQueryable_1_Boolean_String_QueryRequestOptions_CosmosLinqSerializerOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var requestOptions = new QueryRequestOptions();
            var serializerOptions = new CosmosLinqSerializerOptions();

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            Assert.Throws<NotImplementedException>(
                () => target.GetItemLinqQueryable<object>(Any.Boolean(), Any.String(), requestOptions,
                    serializerOptions));
        }
    }

    public class GetItemQueryIterator_1_QueryDefinition_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var queryDefinition = new QueryDefinition("SELECT * FROM c");
            string continuationToken = Any.String();
            var requestOptions = new QueryRequestOptions();

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            Assert.Throws<NotImplementedException>(
                () => target.GetItemQueryIterator<object>(queryDefinition, continuationToken, requestOptions));
        }
    }

    public class GetItemQueryIterator_1_String_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            string query = "SELECT * FROM c";
            string continuationToken = Any.String();
            var requestOptions = new QueryRequestOptions();

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            Assert.Throws<NotImplementedException>(
                () => target.GetItemQueryIterator<object>(query, continuationToken, requestOptions));
        }
    }

    public class GetItemQueryIterator_1_FeedRange_QueryDefinition_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var feedRange = FeedRange.FromPartitionKey(new PartitionKey(Any.String()));
            var queryDefinition = new QueryDefinition("SELECT * FROM c");
            string continuationToken = Any.String();
            var requestOptions = new QueryRequestOptions();

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            Assert.Throws<NotImplementedException>(
                () => target.GetItemQueryIterator<object>(feedRange, queryDefinition, continuationToken,
                    requestOptions));
        }
    }

    public class GetItemQueryStreamIterator_QueryDefinition_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var queryDefinition = new QueryDefinition("SELECT * FROM c");
            string continuationToken = Any.String();
            var requestOptions = new QueryRequestOptions();

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            Assert.Throws<NotImplementedException>(
                () => target.GetItemQueryStreamIterator(queryDefinition, continuationToken, requestOptions));
        }
    }

    public class GetItemQueryStreamIterator_String_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            string query = "SELECT * FROM c";
            string continuationToken = Any.String();
            var requestOptions = new QueryRequestOptions();

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            Assert.Throws<NotImplementedException>(
                () => target.GetItemQueryStreamIterator(query, continuationToken, requestOptions));
        }
    }

    public class GetItemQueryStreamIterator_FeedRange_QueryDefinition_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var feedRange = FeedRange.FromPartitionKey(new PartitionKey(Any.String()));
            var queryDefinition = new QueryDefinition("SELECT * FROM c");
            string continuationToken = Any.String();
            var requestOptions = new QueryRequestOptions();

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            Assert.Throws<NotImplementedException>(
                () => target.GetItemQueryStreamIterator(feedRange, queryDefinition, continuationToken,
                    requestOptions));
        }
    }

    public class PatchItemAsync_1_String_PartitionKey_IReadOnlyList_PatchItemRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var partitionKey = new PartitionKey(Any.String());
            var patchOperations = new List<PatchOperation>();
            var requestOptions = new PatchItemRequestOptions();
            var cancellationToken = CancellationToken.None;

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            await Assert.ThrowsAsync<NotImplementedException>(
                () => target.PatchItemAsync<object>(Any.String(), partitionKey, patchOperations, requestOptions,
                    cancellationToken));
        }
    }

    public class PatchItemStreamAsync_String_PartitionKey_IReadOnlyList_PatchItemRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var partitionKey = new PartitionKey(Any.String());
            var patchOperations = new List<PatchOperation>();
            var requestOptions = new PatchItemRequestOptions();
            var cancellationToken = CancellationToken.None;

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            await Assert.ThrowsAsync<NotImplementedException>(
                () => target.PatchItemStreamAsync(Any.String(), partitionKey, patchOperations, requestOptions,
                    cancellationToken));
        }
    }

    public class ReadContainerAsync_ContainerRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var requestOptions = new ContainerRequestOptions();
            var cancellationToken = CancellationToken.None;

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            await Assert.ThrowsAsync<NotImplementedException>(
                () => target.ReadContainerAsync(requestOptions, cancellationToken));
        }
    }

    public class ReadContainerStreamAsync_ContainerRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var requestOptions = new ContainerRequestOptions();
            var cancellationToken = CancellationToken.None;

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            await Assert.ThrowsAsync<NotImplementedException>(
                () => target.ReadContainerStreamAsync(requestOptions, cancellationToken));
        }
    }

    public class ReadItemAsync_1_String_PartitionKey_ItemRequestOptions_CancellationToken
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            throw new NotImplementedException();
        }
    }

    public class ReadItemStreamAsync_String_PartitionKey_ItemRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsNotImplementedExceptionAsync()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var requestOptions = new ItemRequestOptions();
            var cancellationToken = CancellationToken.None;

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            await Assert.ThrowsAsync<NotImplementedException>(
                () => target.ReadItemStreamAsync(Any.String(), PartitionKey.Null, requestOptions,
                    cancellationToken));
        }
    }

    public class ReadManyItemsAsync_1_IReadOnlyList_ReadManyRequestOptions_CancellationToken
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            throw new NotImplementedException();
        }
    }

    public class ReadManyItemsStreamAsync_IReadOnlyList_ReadManyRequestOptions_CancellationToken
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            throw new NotImplementedException();
        }
    }

    public class ReadThroughputAsync_CancellationToken
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            throw new NotImplementedException();
        }
    }

    public class ReadThroughputAsync_RequestOptions_CancellationToken
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            throw new NotImplementedException();
        }
    }

    public class ReplaceContainerAsync_ContainerProperties_ContainerRequestOptions_CancellationToken
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            throw new NotImplementedException();
        }
    }

    public class ReplaceContainerStreamAsync_ContainerProperties_ContainerRequestOptions_CancellationToken
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            throw new NotImplementedException();
        }
    }

    public class ReplaceItemAsync_1_T_String_PartitionKey_ItemRequestOptions_CancellationToken
    {
        [Fact]
        public void ThrowsCosmosExceptionWhenItemDoesNotExist()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void ReplacesExistingItem()
        {
            throw new NotImplementedException();
        }
    }

    public class ReplaceItemStreamAsync_Stream_String_PartitionKey_ItemRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            string id = Any.String();
            var partitionKey = PartitionKey.Null;
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = CancellationToken.None;

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            await Assert.ThrowsAsync<NotImplementedException>(
                () =>
                {
                    using (var stream = new MemoryStream())
                    {
                        return target.ReplaceItemStreamAsync(stream, id, partitionKey, requestOptions,
                            cancellationToken);
                    }
                });
        }
    }

    public class ReplaceThroughputAsync_Int32_RequestOptions_CancellationToken
    {
    }

    public class ReplaceThroughputAsync_ThroughputProperties_RequestOptions_CancellationToken
    {
    }

    public class UpsertItemAsync_1_T_PartitionKey_ItemRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var item = new { Name = Any.String() };
            var partitionKey = PartitionKey.Null;
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = CancellationToken.None;

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            await Assert.ThrowsAsync<NotImplementedException>(
                () => target.UpsertItemAsync(item, partitionKey, requestOptions, cancellationToken));
        }
    }

    public class UpsertItemStreamAsync_Stream_PartitionKey_ItemRequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsNotImplementedException()
        {
            var database = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var partitionKey = PartitionKey.Null;
            var requestOptions = new ItemRequestOptions();
            var cancellationToken = CancellationToken.None;

            var target = new InMemoryContainer(Any.Int32(), database, new ContainerProperties());

            await Assert.ThrowsAsync<NotImplementedException>(
                () =>
                {
                    using (var stream = new MemoryStream())
                    {
                        return target.UpsertItemStreamAsync(stream, partitionKey, requestOptions,
                            cancellationToken);
                    }
                });
        }
    }

    private class TestingInMemoryCosmosClient : InMemoryCosmosClient
    {
        public string MockDatabaseId { get; }

        public string MockContainerId { get; }

        public Mock<InMemoryDatabase> MockDatabase { get; }

        public Mock<InMemoryContainer> MockContainer { get; }

        public TestingInMemoryCosmosClient(CosmosClientOptions? clientOptions = null)
            : base(clientOptions)
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

    private class Thing
    {
        [JsonProperty("id")]
        public required Guid Id { get; set; }

        [JsonProperty("name")]
        public required string Name { get; set; }
    }
}
