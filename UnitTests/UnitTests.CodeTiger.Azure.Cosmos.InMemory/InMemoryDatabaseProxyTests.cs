using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using CodeTiger.Azure.Cosmos.InMemory;
using Microsoft.Azure.Cosmos;
using Moq;
using Xunit;

namespace UnitTests.CodeTiger.Azure.Cosmos.InMemory;

public class InMemoryDatabaseProxyTests
{
    public class Constructor_String_InMemoryCosmosClient
    {
        [Fact]
        public void SetsIdToProvidedId()
        {
            string expected = Any.String();

            var target = new InMemoryDatabaseProxy(expected, new TestingInMemoryCosmosClient());

            Assert.Equal(expected, target.Id);
        }

        [Fact]
        public void SetsClientToProvidedClient()
        {
            var expected = new TestingInMemoryCosmosClient();

            var target = new InMemoryDatabaseProxy(Any.String(), expected);

            Assert.Same(expected, target.Client);
        }
    }

    public class CreateClientEncryptionKeyAsync_ClientEncryptionKeyProperties_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

            var clientEncryptionKeyProperties = new ClientEncryptionKeyProperties(Any.String(), Any.String(),
                Array.Empty<byte>(),
                new EncryptionKeyWrapMetadata(Any.String(), Any.String(), Any.String(), Any.String()));
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.CreateClientEncryptionKeyAsync(clientEncryptionKeyProperties, requestOptions,
                    cancellationToken));
        }

        [Fact]
        public void CallsRealDatabaseWhenDatabaseExists()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabaseProxy(testingCosmosClient.MockDatabaseId, testingCosmosClient);

            var clientEncryptionKeyProperties = new ClientEncryptionKeyProperties(Any.String(), Any.String(),
                Array.Empty<byte>(),
                new EncryptionKeyWrapMetadata(Any.String(), Any.String(), Any.String(), Any.String()));
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = target.CreateClientEncryptionKeyAsync(clientEncryptionKeyProperties, requestOptions,
                cancellationToken);

            testingCosmosClient.MockDatabase.Verify(
                x => x.CreateClientEncryptionKeyAsync(clientEncryptionKeyProperties, requestOptions,
                    cancellationToken),
                Times.Once());
        }
    }

    public class CreateContainerAsync_ContainerProperties_ThroughputProperties_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

            var containerProperties = new ContainerProperties();
            var throughputProperties = ThroughputProperties.CreateAutoscaleThroughput(Any.Int32());
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.CreateContainerAsync(containerProperties, throughputProperties, requestOptions,
                    cancellationToken));
        }

        [Fact]
        public void CallsRealDatabaseWhenDatabaseExists()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabaseProxy(testingCosmosClient.MockDatabaseId, testingCosmosClient);

            var containerProperties = new ContainerProperties();
            var throughputProperties = ThroughputProperties.CreateAutoscaleThroughput(Any.Int32());
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = target.CreateContainerAsync(containerProperties, throughputProperties, requestOptions,
                cancellationToken);

            testingCosmosClient.MockDatabase.Verify(
                x => x.CreateContainerAsync(containerProperties, throughputProperties, requestOptions,
                    cancellationToken),
                Times.Once());
        }
    }

    public class CreateContainerAsync_ContainerProperties_Int32_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

            var containerProperties = new ContainerProperties();
            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.CreateContainerAsync(containerProperties, throughput, requestOptions,
                    cancellationToken));
        }

        [Fact]
        public void CallsRealDatabaseWhenDatabaseExists()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabaseProxy(testingCosmosClient.MockDatabaseId, testingCosmosClient);

            var containerProperties = new ContainerProperties();
            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = target.CreateContainerAsync(containerProperties, throughput, requestOptions, cancellationToken);

            testingCosmosClient.MockDatabase.Verify(
                x => x.CreateContainerAsync(containerProperties, throughput, requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class CreateContainerAsync_String_String_Int32_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

            string id = Any.String();
            string partitionKeyPath = Any.String();
            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.CreateContainerAsync(id, partitionKeyPath, throughput, requestOptions,
                    cancellationToken));
        }

        [Fact]
        public void CallsRealDatabaseWhenDatabaseExists()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabaseProxy(testingCosmosClient.MockDatabaseId, testingCosmosClient);

            string id = Any.String();
            string partitionKeyPath = Any.String();
            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = target.CreateContainerAsync(id, partitionKeyPath, throughput, requestOptions, cancellationToken);

            testingCosmosClient.MockDatabase.Verify(
                x => x.CreateContainerAsync(id, partitionKeyPath, throughput, requestOptions, cancellationToken),
                Times.Once());
        }
    }

    [SuppressMessage("CodeTiger.Layout", "CT3531:Lines should not exceed the maximum length of 115",
        Justification = "Allowed, to be able to contain the entire method signature.")]
    public class CreateContainerIfNotExistsAsync_ContainerProperties_ThroughputProperties_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

            var containerProperties = new ContainerProperties();
            var throughputProperties = ThroughputProperties.CreateAutoscaleThroughput(Any.Int32());
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.CreateContainerIfNotExistsAsync(containerProperties, throughputProperties,
                    requestOptions, cancellationToken));
        }

        [Fact]
        public void CallsRealDatabaseWhenDatabaseExists()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabaseProxy(testingCosmosClient.MockDatabaseId, testingCosmosClient);

            var containerProperties = new ContainerProperties();
            var throughputProperties = ThroughputProperties.CreateAutoscaleThroughput(Any.Int32());
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = target.CreateContainerIfNotExistsAsync(containerProperties, throughputProperties, requestOptions,
                cancellationToken);

            testingCosmosClient.MockDatabase.Verify(
                x => x.CreateContainerIfNotExistsAsync(containerProperties, throughputProperties, requestOptions,
                    cancellationToken),
                Times.Once());
        }
    }

    public class CreateContainerIfNotExistsAsync_ContainerProperties_Int32_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

            var containerProperties = new ContainerProperties();
            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.CreateContainerIfNotExistsAsync(containerProperties, throughput, requestOptions,
                    cancellationToken));
        }

        [Fact]
        public void CallsRealDatabaseWhenDatabaseExists()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabaseProxy(testingCosmosClient.MockDatabaseId, testingCosmosClient);

            var containerProperties = new ContainerProperties();
            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = target.CreateContainerIfNotExistsAsync(containerProperties, throughput, requestOptions,
                cancellationToken);

            testingCosmosClient.MockDatabase.Verify(
                x => x.CreateContainerIfNotExistsAsync(containerProperties, throughput, requestOptions,
                    cancellationToken),
                Times.Once());
        }
    }

    public class CreateContainerIfNotExistsAsync_String_String_Int32_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

            string id = Any.String();
            string partitionKeyPath = Any.String();
            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.CreateContainerIfNotExistsAsync(id, partitionKeyPath, throughput, requestOptions,
                    cancellationToken));
        }

        [Fact]
        public void CallsRealDatabaseWhenDatabaseExists()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabaseProxy(testingCosmosClient.MockDatabaseId, testingCosmosClient);

            string id = Any.String();
            string partitionKeyPath = Any.String();
            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = target.CreateContainerIfNotExistsAsync(id, partitionKeyPath, throughput, requestOptions,
                cancellationToken);

            testingCosmosClient.MockDatabase.Verify(
                x => x.CreateContainerIfNotExistsAsync(id, partitionKeyPath, throughput, requestOptions,
                    cancellationToken),
                Times.Once());
        }
    }

    [SuppressMessage("CodeTiger.Layout", "CT3531:Lines should not exceed the maximum length of 115",
        Justification = "Allowed, to be able to contain the entire method signature.")]
    public class CreateContainerStreamAsync_ContainerProperties_ThroughputProperties_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

            var containerProperties = new ContainerProperties();
            var throughputProperties = ThroughputProperties.CreateAutoscaleThroughput(Any.Int32());
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.CreateContainerStreamAsync(containerProperties, throughputProperties, requestOptions,
                    cancellationToken));
        }

        [Fact]
        public void CallsRealDatabaseWhenDatabaseExists()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabaseProxy(testingCosmosClient.MockDatabaseId, testingCosmosClient);

            var containerProperties = new ContainerProperties();
            var throughputProperties = ThroughputProperties.CreateAutoscaleThroughput(Any.Int32());
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = target.CreateContainerStreamAsync(containerProperties, throughputProperties, requestOptions,
                cancellationToken);

            testingCosmosClient.MockDatabase.Verify(
                x => x.CreateContainerStreamAsync(containerProperties, throughputProperties, requestOptions,
                    cancellationToken),
                Times.Once());
        }
    }

    public class CreateContainerStreamAsync_ContainerProperties_Int32_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

            var containerProperties = new ContainerProperties();
            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.CreateContainerStreamAsync(containerProperties, throughput, requestOptions,
                    cancellationToken));
        }

        [Fact]
        public void CallsRealDatabaseWhenDatabaseExists()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabaseProxy(testingCosmosClient.MockDatabaseId, testingCosmosClient);

            var containerProperties = new ContainerProperties();
            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = target.CreateContainerStreamAsync(containerProperties, throughput, requestOptions,
                cancellationToken);

            testingCosmosClient.MockDatabase.Verify(
                x => x.CreateContainerStreamAsync(containerProperties, throughput, requestOptions,
                    cancellationToken),
                Times.Once());
        }
    }

    public class CreateUserAsync_String_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

            string id = Any.String();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.CreateUserAsync(id, requestOptions, cancellationToken));
        }

        [Fact]
        public void CallsRealDatabaseWhenDatabaseExists()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabaseProxy(testingCosmosClient.MockDatabaseId, testingCosmosClient);

            string id = Any.String();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = target.CreateUserAsync(id, requestOptions, cancellationToken);

            testingCosmosClient.MockDatabase.Verify(
                x => x.CreateUserAsync(id, requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class DefineContainer_String_String
    {
        [Fact]
        public void ReturnsContainerBuilder()
        {
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

            string name = Any.String();
            string partitionKeyPath = Any.String();

            var actual = target.DefineContainer(name, partitionKeyPath);

            Assert.NotNull(actual);
        }
    }

    public class DeleteAsync_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(() => target.DeleteAsync(requestOptions, cancellationToken));
        }

        [Fact]
        public void CallsRealDatabaseWhenDatabaseExists()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabaseProxy(testingCosmosClient.MockDatabaseId, testingCosmosClient);

            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = target.DeleteAsync(requestOptions, cancellationToken);

            testingCosmosClient.MockDatabase.Verify(
                x => x.DeleteAsync(requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class DeleteStreamAsync_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.DeleteStreamAsync(requestOptions, cancellationToken));
        }

        [Fact]
        public void CallsRealDatabaseWhenDatabaseExists()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabaseProxy(testingCosmosClient.MockDatabaseId, testingCosmosClient);

            var clientEncryptionKeyProperties = new ClientEncryptionKeyProperties(Any.String(), Any.String(),
                Array.Empty<byte>(),
                new EncryptionKeyWrapMetadata(Any.String(), Any.String(), Any.String(), Any.String()));
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = target.DeleteStreamAsync(requestOptions, cancellationToken);

            testingCosmosClient.MockDatabase.Verify(
                x => x.DeleteStreamAsync(requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class GetClientEncryptionKey_String
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

            Assert.Throws<NotImplementedException>(() => target.GetClientEncryptionKey(Any.String()));
        }
    }

    public class GetClientEncryptionKeyQueryIterator_QueryDefinition_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

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
        public void ReturnsContainer()
        {
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

            string id = Any.String();

            var actual = target.GetContainer(id);

            Assert.Same(id, actual.Id);
        }
    }

    public class GetContainerQueryIterator_1_QueryDefinition_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

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
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

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
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

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
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

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
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

            string id = Any.String();

            Assert.Throws<NotImplementedException>(() => target.GetUser(id));
        }
    }

    public class GetUserQueryIterator_1_String_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

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
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

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
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(() => target.ReadAsync(requestOptions, cancellationToken));
        }

        [Fact]
        public void CallsRealDatabaseWhenDatabaseExists()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabaseProxy(testingCosmosClient.MockDatabaseId, testingCosmosClient);

            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = target.ReadAsync(requestOptions,
                cancellationToken);

            testingCosmosClient.MockDatabase.Verify(
                x => x.ReadAsync(requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class ReadStreamAsync_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReadStreamAsync(requestOptions, cancellationToken));
        }

        [Fact]
        public void CallsRealDatabaseWhenDatabaseExists()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabaseProxy(testingCosmosClient.MockDatabaseId, testingCosmosClient);

            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = target.ReadStreamAsync(requestOptions, cancellationToken);

            testingCosmosClient.MockDatabase.Verify(
                x => x.ReadStreamAsync(requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class ReadThroughputAsync_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(() => target.ReadThroughputAsync(cancellationToken));
        }

        [Fact]
        public void CallsRealDatabaseWhenDatabaseExists()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabaseProxy(testingCosmosClient.MockDatabaseId, testingCosmosClient);

            var cancellationToken = new CancellationToken(false);

            _ = target.ReadThroughputAsync(cancellationToken);

            testingCosmosClient.MockDatabase.Verify(x => x.ReadThroughputAsync(cancellationToken), Times.Once());
        }
    }

    public class ReadThroughputAsync_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReadThroughputAsync(requestOptions, cancellationToken));
        }

        [Fact]
        public void CallsRealDatabaseWhenDatabaseExists()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabaseProxy(testingCosmosClient.MockDatabaseId, testingCosmosClient);

            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = target.ReadThroughputAsync(requestOptions, cancellationToken);

            testingCosmosClient.MockDatabase.Verify(
                x => x.ReadThroughputAsync(requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class ReplaceThroughputAsync_ThroughputProperties_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

            var throughputProperties = ThroughputProperties.CreateAutoscaleThroughput(Any.Int32());
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReplaceThroughputAsync(throughputProperties, requestOptions, cancellationToken));
        }

        [Fact]
        public void CallsRealDatabaseWhenDatabaseExists()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabaseProxy(testingCosmosClient.MockDatabaseId, testingCosmosClient);

            var throughputProperties = ThroughputProperties.CreateAutoscaleThroughput(Any.Int32());
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = target.ReplaceThroughputAsync(throughputProperties, requestOptions, cancellationToken);

            testingCosmosClient.MockDatabase.Verify(
                x => x.ReplaceThroughputAsync(throughputProperties, requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class ReplaceThroughputAsync_Int32_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.ReplaceThroughputAsync(throughput, requestOptions, cancellationToken));
        }

        [Fact]
        public void CallsRealDatabaseWhenDatabaseExists()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabaseProxy(testingCosmosClient.MockDatabaseId, testingCosmosClient);

            int throughput = Any.Int32();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = target.ReplaceThroughputAsync(throughput, requestOptions, cancellationToken);

            testingCosmosClient.MockDatabase.Verify(
                x => x.ReplaceThroughputAsync(throughput, requestOptions, cancellationToken),
                Times.Once());
        }
    }

    public class UpsertUserAsync_String_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseDoesNotExist()
        {
            var target = new InMemoryDatabaseProxy(Any.String(), new TestingInMemoryCosmosClient());

            string id = Any.String();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            await Assert.ThrowsAsync<CosmosException>(
                () => target.UpsertUserAsync(id, requestOptions, cancellationToken));
        }

        [Fact]
        public void CallsRealDatabaseWhenDatabaseExists()
        {
            var testingCosmosClient = new TestingInMemoryCosmosClient();
            var target = new InMemoryDatabaseProxy(testingCosmosClient.MockDatabaseId, testingCosmosClient);

            string id = Any.String();
            var requestOptions = new RequestOptions();
            var cancellationToken = new CancellationToken(false);

            _ = target.UpsertUserAsync(id, requestOptions, cancellationToken);

            testingCosmosClient.MockDatabase.Verify(
                x => x.UpsertUserAsync(id, requestOptions, cancellationToken),
                Times.Once());
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
