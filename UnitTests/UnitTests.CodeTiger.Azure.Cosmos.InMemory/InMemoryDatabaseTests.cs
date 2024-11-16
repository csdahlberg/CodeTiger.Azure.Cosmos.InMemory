using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using CodeTiger.Azure.Cosmos.InMemory;
using Microsoft.Azure.Cosmos;
using Moq;
using Xunit;

namespace UnitTests.CodeTiger.Azure.Cosmos.InMemory;

public class InMemoryDatabaseTests
{
    public class Constructor_InMemoryCosmosClient_String
    {
        [Fact]
        public void SetsClientToProvidedClient()
        {
            var client = new InMemoryCosmosClient(
                new CosmosClientOptions { Serializer = new InMemoryCosmosJsonDotNetSerializer() });

            var target = new InMemoryDatabase(client, Any.String());

            Assert.Equal(client, target.Client);
        }

        [Fact]
        public void SetsIdToProvidedId()
        {
            var client = new InMemoryCosmosClient(
                new CosmosClientOptions { Serializer = new InMemoryCosmosJsonDotNetSerializer() });
            string id = Any.String();

            var target = new InMemoryDatabase(client, id);

            Assert.Equal(id, target.Id);
        }
    }

    public class CreateClientEncryptionKeyAsync_ClientEncryptionKeyProperties_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var clientEncryptionKeyProperties = new ClientEncryptionKeyProperties(Any.String(), Any.String(),
                Array.Empty<byte>(),
                new EncryptionKeyWrapMetadata(Any.String(), Any.String(), Any.String(), Any.String()));
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<NotImplementedException>(
                () => target.CreateClientEncryptionKeyAsync(clientEncryptionKeyProperties, requestOptions,
                    cancellationToken));
        }
    }

    public class CreateContainerAsync_ContainerProperties_ThroughputProperties_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ReturnsContainerWhenNewContainerIsCreated()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabase(testingCosmosClient, testingCosmosClient.MockDatabaseId);

            var containerProperties = new ContainerProperties(Any.String(), Any.PartitionKey());
            var throughputProperties = ThroughputProperties.CreateAutoscaleThroughput(Any.Int32());
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            var actual = await target.CreateContainerAsync(containerProperties, throughputProperties,
                requestOptions, cancellationToken);

            Assert.Equal(containerProperties.Id, actual.Resource.Id);
        }

        [Fact]
        public async Task AddsContainerToDictionaryWhenNewContainerIsCreated()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabase(testingCosmosClient, testingCosmosClient.MockDatabaseId);

            var containerProperties = new ContainerProperties(Any.String(), Any.PartitionKey());
            var throughputProperties = ThroughputProperties.CreateAutoscaleThroughput(Any.Int32());
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            var actual = await target.CreateContainerAsync(containerProperties, throughputProperties,
                requestOptions, cancellationToken);

            Assert.Contains(target.InMemoryContainers, x => x.Value.Id == containerProperties.Id);
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenAttemptingToCreateExistingContainer()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabase(testingCosmosClient, testingCosmosClient.MockDatabaseId);

            var containerProperties = new ContainerProperties(Any.String(), Any.PartitionKey());
            var throughputProperties = ThroughputProperties.CreateAutoscaleThroughput(Any.Int32());
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = await target.CreateContainerAsync(containerProperties, throughputProperties,
                requestOptions, cancellationToken);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.CreateContainerAsync(containerProperties, throughputProperties, requestOptions,
                    cancellationToken));
        }
    }

    public class CreateContainerAsync_ContainerProperties_Int32_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ReturnsContainerWhenNewContainerIsCreated()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabase(testingCosmosClient, testingCosmosClient.MockDatabaseId);

            var containerProperties = new ContainerProperties(Any.String(), Any.PartitionKey());
            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            var actual = await target.CreateContainerAsync(containerProperties, throughput, requestOptions,
                cancellationToken);

            Assert.Equal(containerProperties.Id, actual.Resource.Id);
        }

        [Fact]
        public async Task AddsContainerToDictionaryWhenNewContainerIsCreated()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabase(testingCosmosClient, testingCosmosClient.MockDatabaseId);

            var containerProperties = new ContainerProperties(Any.String(), Any.PartitionKey());
            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            var actual = await target.CreateContainerAsync(containerProperties, throughput, requestOptions,
                cancellationToken);

            Assert.Contains(target.InMemoryContainers, x => x.Value.Id == containerProperties.Id);
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenAttemptingToCreateExistingContainer()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabase(testingCosmosClient, testingCosmosClient.MockDatabaseId);

            var containerProperties = new ContainerProperties(Any.String(), Any.PartitionKey());
            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = await target.CreateContainerAsync(containerProperties, throughput, requestOptions,
                cancellationToken);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.CreateContainerAsync(containerProperties, throughput, requestOptions,
                    cancellationToken));
        }
    }

    public class CreateContainerAsync_String_String_Int32_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ReturnsContainerWhenNewContainerIsCreated()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabase(testingCosmosClient, testingCosmosClient.MockDatabaseId);

            string id = Any.String();
            string partitionKeyPath = Any.PartitionKey();
            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            var actual = await target.CreateContainerAsync(id, partitionKeyPath, throughput, requestOptions,
                cancellationToken);

            Assert.Equal(id, actual.Resource.Id);
        }

        [Fact]
        public async Task AddsContainerToDictionaryWhenNewContainerIsCreated()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabase(testingCosmosClient, testingCosmosClient.MockDatabaseId);

            string id = Any.String();
            string partitionKeyPath = Any.PartitionKey();
            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            var actual = await target.CreateContainerAsync(id, partitionKeyPath, throughput, requestOptions,
                cancellationToken);

            Assert.Contains(target.InMemoryContainers, x => x.Value.Id == id);
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenAttemptingToCreateExistingContainer()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabase(testingCosmosClient, testingCosmosClient.MockDatabaseId);

            string id = Any.String();
            string partitionKeyPath = Any.PartitionKey();
            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = await target.CreateContainerAsync(id, partitionKeyPath, throughput, requestOptions,
                cancellationToken);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.CreateContainerAsync(id, partitionKeyPath, throughput, requestOptions,
                    cancellationToken));
        }
    }

    [SuppressMessage("CodeTiger.Layout", "CT3531:Lines should not exceed the maximum length of 115",
        Justification = "Allowed, to be able to contain the entire method signature.")]
    public class CreateContainerIfNotExistsAsync_ContainerProperties_ThroughputProperties_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ReturnsContainerWhenNewContainerIsCreated()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabase(testingCosmosClient, testingCosmosClient.MockDatabaseId);

            var containerProperties = new ContainerProperties(Any.String(), Any.PartitionKey());
            var throughputProperties = ThroughputProperties.CreateAutoscaleThroughput(Any.Int32());
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            var actual = await target.CreateContainerIfNotExistsAsync(containerProperties, throughputProperties,
                requestOptions, cancellationToken);

            Assert.Equal(containerProperties.Id, actual.Resource.Id);
        }

        [Fact]
        public async Task AddsContainerToDictionaryWhenNewContainerIsCreated()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabase(testingCosmosClient, testingCosmosClient.MockDatabaseId);

            var containerProperties = new ContainerProperties(Any.String(), Any.PartitionKey());
            var throughputProperties = ThroughputProperties.CreateAutoscaleThroughput(Any.Int32());
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            var actual = await target.CreateContainerIfNotExistsAsync(containerProperties, throughputProperties,
                requestOptions, cancellationToken);

            Assert.Contains(target.InMemoryContainers, x => x.Value.Id == containerProperties.Id);
        }

        [Fact]
        public async Task ReturnsContainerWhenAttemptingToCreateExistingContainer()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabase(testingCosmosClient, testingCosmosClient.MockDatabaseId);

            var containerProperties = new ContainerProperties(Any.String(), Any.PartitionKey());
            var throughputProperties = ThroughputProperties.CreateAutoscaleThroughput(Any.Int32());
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = await target.CreateContainerIfNotExistsAsync(containerProperties, throughputProperties,
                requestOptions, cancellationToken);

            var actual = await target.CreateContainerIfNotExistsAsync(containerProperties, throughputProperties,
                requestOptions, cancellationToken);

            Assert.Equal(containerProperties.Id, actual.Resource.Id);
        }
    }

    public class CreateContainerIfNotExistsAsync_ContainerProperties_Int32_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ReturnsContainerWhenNewContainerIsCreated()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabase(testingCosmosClient, testingCosmosClient.MockDatabaseId);

            var containerProperties = new ContainerProperties(Any.String(), Any.PartitionKey());
            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            var actual = await target.CreateContainerIfNotExistsAsync(containerProperties, throughput,
                requestOptions, cancellationToken);

            Assert.Equal(containerProperties.Id, actual.Resource.Id);
        }

        [Fact]
        public async Task AddsContainerToDictionaryWhenNewContainerIsCreated()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabase(testingCosmosClient, testingCosmosClient.MockDatabaseId);

            var containerProperties = new ContainerProperties(Any.String(), Any.PartitionKey());
            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            var actual = await target.CreateContainerIfNotExistsAsync(containerProperties, throughput,
                requestOptions, cancellationToken);

            Assert.Contains(target.InMemoryContainers, x => x.Value.Id == containerProperties.Id);
        }

        [Fact]
        public async Task ReturnsContainerWhenAttemptingToCreateExistingContainer()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabase(testingCosmosClient, testingCosmosClient.MockDatabaseId);

            var containerProperties = new ContainerProperties(Any.String(), Any.PartitionKey());
            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = await target.CreateContainerIfNotExistsAsync(containerProperties, throughput, requestOptions,
                cancellationToken);

            var actual = await target.CreateContainerIfNotExistsAsync(containerProperties, throughput,
                requestOptions, cancellationToken);

            Assert.Equal(containerProperties.Id, actual.Resource.Id);
        }
    }

    public class CreateContainerIfNotExistsAsync_String_String_Int32_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ReturnsContainerWhenNewContainerIsCreated()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabase(testingCosmosClient, testingCosmosClient.MockDatabaseId);

            string id = Any.String();
            string partitionKeyPath = Any.PartitionKey();
            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            var actual = await target.CreateContainerIfNotExistsAsync(id, partitionKeyPath, throughput,
                requestOptions, cancellationToken);

            Assert.Equal(id, actual.Resource.Id);
        }

        [Fact]
        public async Task AddsContainerToDictionaryWhenNewContainerIsCreated()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabase(testingCosmosClient, testingCosmosClient.MockDatabaseId);

            string id = Any.String();
            string partitionKeyPath = Any.PartitionKey();
            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            var actual = await target.CreateContainerIfNotExistsAsync(id, partitionKeyPath, throughput,
                requestOptions, cancellationToken);

            Assert.Contains(target.InMemoryContainers, x => x.Value.Id == id);
        }

        [Fact]
        public async Task ReturnsContainerWhenAttemptingToCreateExistingContainer()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabase(testingCosmosClient, testingCosmosClient.MockDatabaseId);

            string id = Any.String();
            string partitionKeyPath = Any.PartitionKey();
            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = await target.CreateContainerIfNotExistsAsync(id, partitionKeyPath, throughput, requestOptions,
                cancellationToken);

            var actual = await target.CreateContainerIfNotExistsAsync(id, partitionKeyPath, throughput,
                requestOptions, cancellationToken);

            Assert.Equal(id, actual.Resource.Id);
        }
    }

    [SuppressMessage("CodeTiger.Layout", "CT3531:Lines should not exceed the maximum length of 115",
        Justification = "Allowed, to be able to contain the entire method signature.")]
    public class CreateContainerStreamAsync_ContainerProperties_ThroughputProperties_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var containerProperties = new ContainerProperties();
            var throughputProperties = ThroughputProperties.CreateAutoscaleThroughput(Any.Int32());
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<NotImplementedException>(
                () => target.CreateContainerStreamAsync(containerProperties, throughputProperties, requestOptions,
                    cancellationToken));
        }
    }

    public class CreateContainerStreamAsync_ContainerProperties_Int32_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var containerProperties = new ContainerProperties();
            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<NotImplementedException>(
                () => target.CreateContainerStreamAsync(containerProperties, throughput, requestOptions,
                    cancellationToken));
        }
    }

    public class CreateUserAsync_String_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            string id = Any.String();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<NotImplementedException>(
                () => target.CreateUserAsync(id, requestOptions, cancellationToken));
        }
    }

    public class DefineContainer_String_String
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            string name = Any.String();
            string partitionKeyPath = Any.String();

            Assert.Throws<NotImplementedException>(() => target.DefineContainer(name, partitionKeyPath));
        }
    }

    public class DeleteAsync_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ReturnsContainerWhenExistingContainerIsDeleted()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();

            string id = Any.String();
            _ = await testingCosmosClient.CreateDatabaseAsync(id);

            var target = testingCosmosClient.GetDatabase(id);

            var actual = await target.DeleteAsync(new RequestOptions(), new CancellationToken(false));

            Assert.Equal(id, actual.Resource.Id);
        }

        [Fact]
        public async Task RemovesExistingDatabaseFromDictionary()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();

            string id = Any.String();
            _ = await testingCosmosClient.CreateDatabaseAsync(id);

            var target = testingCosmosClient.GetDatabase(id);

            _ = await target.DeleteAsync(new RequestOptions(), new CancellationToken(false));

            Assert.DoesNotContain(testingCosmosClient.InMemoryDatabases, x => x.Value.Id == id);
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenAttemptingToDeleteNonExistentContainer()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();

            var target = testingCosmosClient.GetDatabase(Any.String());

            await Assert.ThrowsAsync<CosmosException>(
                () => target.DeleteAsync(new RequestOptions(), new CancellationToken(false)));
        }
    }

    public class DeleteStreamAsync_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<NotImplementedException>(
                () => target.DeleteStreamAsync(requestOptions, cancellationToken));
        }
    }

    public class GetClientEncryptionKey_String
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            string id = Any.String();

            Assert.Throws<NotImplementedException>(() => target.GetClientEncryptionKey(id));
        }
    }

    public class GetClientEncryptionKeyQueryIterator_QueryDefinition_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var queryDefinition = new QueryDefinition("SELECT * FROM c");
            string continuationToken = Any.String();
            var requestOptions = new QueryRequestOptions();

            Assert.Throws<NotImplementedException>(
                () => target.GetClientEncryptionKeyQueryIterator(queryDefinition, continuationToken,
                    requestOptions));
        }
    }

    public class GetContainer_String
    {
        [Fact]
        public void ReturnsContainerProxy()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabase(testingCosmosClient, Any.String());

            var actual = target.GetContainer(Any.String());

            Assert.IsType<InMemoryContainerProxy>(actual);
        }

        [Fact]
        public void ReturnsContainerWithCorrectId()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabase(testingCosmosClient, Any.String());

            string id = Any.String();

            var actual = target.GetContainer(id);

            Assert.Equal(id, actual.Id);
        }
    }

    public class GetContainerQueryIterator_1_QueryDefinition_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var queryDefinition = new QueryDefinition("SELECT * FROM c");
            string continuationToken = Any.String();
            var requestOptions = new QueryRequestOptions();

            Assert.Throws<NotImplementedException>(
                () => target.GetContainerQueryIterator<object>(queryDefinition, continuationToken,
                    requestOptions));
        }
    }

    public class GetContainerQueryIterator_1_String_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            string queryText = Any.String();
            string continuationToken = Any.String();
            var requestOptions = new QueryRequestOptions();

            Assert.Throws<NotImplementedException>(
                () => target.GetContainerQueryIterator<object>(queryText, continuationToken, requestOptions));
        }
    }

    public class GetContainerQueryStreamIterator_QueryDefinition_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var queryDefinition = new QueryDefinition("SELECT * FROM c");
            string continuationToken = Any.String();
            var requestOptions = new QueryRequestOptions();

            Assert.Throws<NotImplementedException>(
                () => target.GetContainerQueryStreamIterator(queryDefinition, continuationToken, requestOptions));
        }
    }

    public class GetContainerQueryStreamIterator_String_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            string queryText = Any.String();
            string continuationToken = Any.String();
            var requestOptions = new QueryRequestOptions();

            Assert.Throws<NotImplementedException>(
                () => target.GetContainerQueryStreamIterator(queryText, continuationToken, requestOptions));
        }
    }

    public class GetUser_String
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            string id = Any.String();

            Assert.Throws<NotImplementedException>(() => target.GetUser(id));
        }
    }

    public class GetUserQueryIterator_1_String_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            string queryText = Any.String();
            string continuationToken = Any.String();
            var requestOptions = new QueryRequestOptions();

            Assert.Throws<NotImplementedException>(
                () => target.GetUserQueryIterator<object>(queryText, continuationToken, requestOptions));
        }
    }

    public class GetUserQueryIterator_1_QueryDefinition_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var queryDefinition = new QueryDefinition("SELECT * FROM c");
            string continuationToken = Any.String();
            var requestOptions = new QueryRequestOptions();

            Assert.Throws<NotImplementedException>(
                () => target.GetUserQueryIterator<object>(queryDefinition, continuationToken, requestOptions));
        }
    }

    public class ReadAsync_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<NotImplementedException>(
                () => target.ReadAsync(requestOptions, cancellationToken));
        }
    }

    public class ReadStreamAsync_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<NotImplementedException>(
                () => target.ReadStreamAsync(requestOptions, cancellationToken));
        }
    }

    public class ReadThroughputAsync_CancellationToken
    {
        [Fact]
        public async Task ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<NotImplementedException>(() => target.ReadThroughputAsync(cancellationToken));
        }
    }

    public class ReadThroughputAsync_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<NotImplementedException>(
                () => target.ReadThroughputAsync(requestOptions, cancellationToken));
        }
    }

    public class ReplaceThroughputAsync_ThroughputProperties_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            var throughputProperties = ThroughputProperties.CreateAutoscaleThroughput(Any.Int32());
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<NotImplementedException>(
                () => target.ReplaceThroughputAsync(throughputProperties, requestOptions, cancellationToken));
        }
    }

    public class ReplaceThroughputAsync_Int32_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<NotImplementedException>(
                () => target.ReplaceThroughputAsync(throughput, requestOptions, cancellationToken));
        }
    }

    public class UpsertUserAsync_String_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabase(new TestingInMemoryCosmosClient(), Any.String());

            string id = Any.String();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<NotImplementedException>(
                () => target.UpsertUserAsync(id, requestOptions, cancellationToken));
        }
    }

    internal class TestingInMemoryCosmosClient : InMemoryCosmosClient
    {
        public string MockDatabaseId { get; }

        public Mock<InMemoryDatabase> MockDatabase { get; }

        public TestingInMemoryCosmosClient()
        {
            MockDatabaseId = Any.String();
            MockDatabase = new Mock<InMemoryDatabase>(this, MockDatabaseId);

            InMemoryDatabases.TryAdd(MockDatabaseId, MockDatabase.Object);
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
