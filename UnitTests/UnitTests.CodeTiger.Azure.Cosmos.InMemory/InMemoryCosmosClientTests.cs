using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using CodeTiger.Azure.Cosmos.InMemory;
using Microsoft.Azure.Cosmos;
using Xunit;

namespace UnitTests.CodeTiger.Azure.Cosmos.InMemory;

public partial class InMemoryCosmosClientTests
{
    [Collection("InMemoryCosmosClient collection")]
    public class Constructor
    {
        [Fact]
        public void UsesDefaultAccountName()
        {
            var target = new InMemoryCosmosClient();

            string actual = Assert.Single(target.Accounts.Keys);

            Assert.Equal(InMemoryCosmosClient.DefaultAccountName, actual);
        }

        [Fact]
        public void UsesDefaultSerializer()
        {
            var target = new InMemoryCosmosClient();

            Assert.IsType<InMemoryCosmosJsonDotNetSerializer>(target.Serializer);
        }
    }

    [Collection("InMemoryCosmosClient collection")]
    public class Constructor_String
    {
        [Fact]
        public void ThrowsArgumentNullExceptionWhenAccountNameIsNull()
        {
            Assert.Throws<ArgumentNullException>("accountName", () => new InMemoryCosmosClient((string)null!));
        }

        [Fact]
        public void ThrowsArgumentExceptionWhenAccountNameIsEmpty()
        {
            Assert.Throws<ArgumentException>("accountName", () => new InMemoryCosmosClient(""));
        }

        [Fact]
        public void UsesProvidedAccountName()
        {
            string expected = Any.String();

            var target = new InMemoryCosmosClient(expected);

            string actual = Assert.Single(target.Accounts.Keys);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UsesDefaultSerializer()
        {
            var target = new InMemoryCosmosClient();

            Assert.IsType<InMemoryCosmosJsonDotNetSerializer>(target.Serializer);
        }
    }

    [Collection("InMemoryCosmosClient collection")]
    public class Constructor_CosmosClientOptions
    {
        [Fact]
        public void UsesDefaultAccountName()
        {
            var clientOptions = new CosmosClientOptions();

            var target = new InMemoryCosmosClient(clientOptions);

            string actual = Assert.Single(target.Accounts.Keys);

            Assert.Equal(InMemoryCosmosClient.DefaultAccountName, actual);
        }

        [Fact]
        public void UsesDefaultSerializerWhenNeitherSerializerNorSerializerOptionsAreProvided()
        {
            var clientOptions = new CosmosClientOptions();

            var target = new InMemoryCosmosClient(clientOptions);

            Assert.IsType<InMemoryCosmosJsonDotNetSerializer>(target.Serializer);
        }

        [Fact]
        public void UsesDefaultSerializerTypeWhenSerializerOptionsAreProvided()
        {
            var clientOptions = new CosmosClientOptions { SerializerOptions = new CosmosSerializationOptions() };

            var target = new InMemoryCosmosClient(clientOptions);

            Assert.IsType<InMemoryCosmosJsonDotNetSerializer>(target.Serializer);
        }

        [Fact]
        public void UsesProvidedSerializer()
        {
            var clientOptions = new CosmosClientOptions { Serializer = new InMemoryCosmosJsonDotNetSerializer() };

            var target = new InMemoryCosmosClient(clientOptions);

            Assert.Same(clientOptions.Serializer, target.Serializer);
        }
    }

    [Collection("InMemoryCosmosClient collection")]
    public class Constructor_String_CosmosClientOptions
    {
        [Fact]
        public void ThrowsArgumentNullExceptionWhenAccountNameIsNull()
        {
            Assert.Throws<ArgumentNullException>("accountName",
                () => new InMemoryCosmosClient(null!, new CosmosClientOptions()));
        }

        [Fact]
        public void ThrowsArgumentExceptionWhenAccountNameIsEmpty()
        {
            Assert.Throws<ArgumentException>("accountName",
                () => new InMemoryCosmosClient("", new CosmosClientOptions()));
        }

        [Fact]
        public void UsesProvidedAccountName()
        {
            string expected = Any.String();

            var target = new InMemoryCosmosClient(expected, new CosmosClientOptions());

            string actual = Assert.Single(target.Accounts.Keys);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void UsesDefaultSerializerWhenNeitherSerializerNorSerializerOptionsAreProvided()
        {
            var clientOptions = new CosmosClientOptions();

            var target = new InMemoryCosmosClient(Any.String(), clientOptions);

            Assert.IsType<InMemoryCosmosJsonDotNetSerializer>(target.Serializer);
        }

        [Fact]
        public void UsesDefaultSerializerTypeWhenSerializerOptionsAreProvided()
        {
            var clientOptions = new CosmosClientOptions { SerializerOptions = new CosmosSerializationOptions() };

            var target = new InMemoryCosmosClient(Any.String(), clientOptions);

            Assert.IsType<InMemoryCosmosJsonDotNetSerializer>(target.Serializer);
        }

        [Fact]
        public void UsesProvidedSerializer()
        {
            var clientOptions = new CosmosClientOptions { Serializer = new InMemoryCosmosJsonDotNetSerializer() };

            var target = new InMemoryCosmosClient(Any.String(), clientOptions);

            Assert.Same(clientOptions.Serializer, target.Serializer);
        }
    }

    public class CreateDatabaseAsync_String_Int32_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsArgumentNullExceptionWhenIdIsNull()
        {
            var target = new InMemoryCosmosClient();

            await Assert.ThrowsAsync<ArgumentNullException>("id",
                () => target.CreateDatabaseAsync(null!, (int?)null, null, CancellationToken.None));
        }

        [Fact]
        public async Task ThrowsArgumentNullExceptionWhenIdIsEmpty()
        {
            var target = new InMemoryCosmosClient();

            await Assert.ThrowsAsync<ArgumentException>("id",
                () => target.CreateDatabaseAsync("", (int?)null, null, CancellationToken.None));
        }

        [Fact]
        public async Task ThrowsNotImplementedExceptionWhenRequestOptionsAreProvided()
        {
            var target = new InMemoryCosmosClient();

            await Assert.ThrowsAsync<NotImplementedException>(
                () => target.CreateDatabaseAsync(Any.String(), (int?)null, new RequestOptions(),
                    CancellationToken.None));
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseAlreadyExists()
        {
            string id = Any.String();

            var target = new InMemoryCosmosClient();

            _ = target.Accounts[InMemoryCosmosClient.DefaultAccountName]
                .TryAdd(id, new InMemoryDatabase(target, id));

            await Assert.ThrowsAsync<CosmosException>(
                () => target.CreateDatabaseAsync(id, (int?)null, null, CancellationToken.None));
        }

        [Fact]
        public async Task ReturnsNewDatabaseWhenItDoesNotAlreadyExist()
        {
            string id = Any.String();

            var target = new InMemoryCosmosClient();

            var actual = await target.CreateDatabaseAsync(id, (int?)null, null, CancellationToken.None);

            Assert.Equal(id, actual.Resource.Id);
        }
    }

    public class CreateDatabaseAsync_String_ThroughputProperties_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsArgumentNullExceptionWhenIdIsNull()
        {
            var target = new InMemoryCosmosClient();

            await Assert.ThrowsAsync<ArgumentNullException>("id",
                () => target.CreateDatabaseAsync(null!, (ThroughputProperties?)null, null,
                    CancellationToken.None));
        }

        [Fact]
        public async Task ThrowsArgumentNullExceptionWhenIdIsEmpty()
        {
            var target = new InMemoryCosmosClient();

            await Assert.ThrowsAsync<ArgumentException>("id",
                () => target.CreateDatabaseAsync("", (ThroughputProperties?)null, null, CancellationToken.None));
        }

        [Fact]
        public async Task ThrowsNotImplementedExceptionWhenRequestOptionsAreProvided()
        {
            var target = new InMemoryCosmosClient();

            await Assert.ThrowsAsync<NotImplementedException>(
                () => target.CreateDatabaseAsync(Any.String(), (ThroughputProperties?)null, new RequestOptions(),
                    CancellationToken.None));
        }

        [Fact]
        public async Task ThrowsCosmosExceptionWhenDatabaseAlreadyExists()
        {
            string id = Any.String();

            var target = new InMemoryCosmosClient();

            _ = target.Accounts[InMemoryCosmosClient.DefaultAccountName]
                .TryAdd(id, new InMemoryDatabase(target, id));

            await Assert.ThrowsAsync<CosmosException>(
                () => target.CreateDatabaseAsync(id, (ThroughputProperties?)null, null, CancellationToken.None));
        }

        [Fact]
        public async Task ReturnsNewDatabaseWhenItDoesNotAlreadyExist()
        {
            string id = Any.String();

            var target = new InMemoryCosmosClient();

            var actual = await target.CreateDatabaseAsync(id, (ThroughputProperties?)null, null,
                CancellationToken.None);

            Assert.Equal(id, actual.Resource.Id);
        }
    }

    public class CreateDatabaseIfNotExistsAsync_String_Int32_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsArgumentNullExceptionWhenIdIsNull()
        {
            var target = new InMemoryCosmosClient();

            await Assert.ThrowsAsync<ArgumentNullException>("id",
                () => target.CreateDatabaseIfNotExistsAsync(null!, (int?)null, null, CancellationToken.None));
        }

        [Fact]
        public async Task ThrowsArgumentNullExceptionWhenIdIsEmpty()
        {
            var target = new InMemoryCosmosClient();

            await Assert.ThrowsAsync<ArgumentException>("id",
                () => target.CreateDatabaseIfNotExistsAsync("", (int?)null, null, CancellationToken.None));
        }

        [Fact]
        public async Task ThrowsNotImplementedExceptionWhenRequestOptionsAreProvided()
        {
            var target = new InMemoryCosmosClient();

            await Assert.ThrowsAsync<NotImplementedException>(
                () => target.CreateDatabaseIfNotExistsAsync(Any.String(), (int?)null, new RequestOptions(),
                    CancellationToken.None));
        }

        [Fact]
        public async Task ReturnsExistingDatabaseWhenDatabaseAlreadyExists()
        {
            string id = Any.String();

            var target = new InMemoryCosmosClient();

            var expected = target.Accounts[InMemoryCosmosClient.DefaultAccountName]
                .GetOrAdd(id, new InMemoryDatabase(target, id));

            var actual = await target.CreateDatabaseIfNotExistsAsync(id, (int?)null, null, CancellationToken.None);

            Assert.Same(expected.Id, actual.Resource.Id);
        }

        [Fact]
        public async Task ReturnsNewDatabaseWhenItDoesNotAlreadyExist()
        {
            string id = Any.String();

            var target = new InMemoryCosmosClient();

            var actual = await target.CreateDatabaseIfNotExistsAsync(id, (int?)null, null, CancellationToken.None);

            Assert.Equal(id, actual.Resource.Id);
        }
    }

    public class CreateDatabaseIfNotExistsAsync_String_ThroughputProperties_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsArgumentNullExceptionWhenIdIsNull()
        {
            var target = new InMemoryCosmosClient();

            await Assert.ThrowsAsync<ArgumentNullException>("id",
                () => target.CreateDatabaseIfNotExistsAsync(null!, (ThroughputProperties?)null, null,
                    CancellationToken.None));
        }

        [Fact]
        public async Task ThrowsArgumentNullExceptionWhenIdIsEmpty()
        {
            var target = new InMemoryCosmosClient();

            await Assert.ThrowsAsync<ArgumentException>("id",
                () => target.CreateDatabaseIfNotExistsAsync("", (ThroughputProperties?)null, null,
                    CancellationToken.None));
        }

        [Fact]
        public async Task ThrowsNotImplementedExceptionWhenRequestOptionsAreProvided()
        {
            var target = new InMemoryCosmosClient();

            await Assert.ThrowsAsync<NotImplementedException>(
                () => target.CreateDatabaseIfNotExistsAsync(Any.String(), (ThroughputProperties?)null,
                    new RequestOptions(), CancellationToken.None));
        }

        [Fact]
        public async Task ReturnsExistingDatabaseWhenDatabaseAlreadyExists()
        {
            string id = Any.String();

            var target = new InMemoryCosmosClient();

            _ = target.Accounts[InMemoryCosmosClient.DefaultAccountName]
                .TryAdd(id, new InMemoryDatabase(target, id));

            var actual = await target.CreateDatabaseIfNotExistsAsync(id, (ThroughputProperties?)null, null,
                CancellationToken.None);

            Assert.Equal(id, actual.Resource.Id);
        }

        [Fact]
        public async Task ReturnsNewDatabaseWhenItDoesNotAlreadyExist()
        {
            string id = Any.String();

            var target = new InMemoryCosmosClient();

            var actual = await target.CreateDatabaseIfNotExistsAsync(id, (ThroughputProperties?)null, null,
                CancellationToken.None);

            Assert.Equal(id, actual.Resource.Id);
        }
    }

    public class CreateDatabaseStreamAsync_DatabaseProperties_Int32_RequestOptions_CancellationToken
    {
        [Fact]
        public async Task ThrowsNotImplementedException()
        {
            var target = new InMemoryCosmosClient();

            await Assert.ThrowsAsync<NotImplementedException>(
                () => target.CreateDatabaseStreamAsync(new DatabaseProperties(), (int?)null, null,
                    CancellationToken.None));
        }
    }

    public class Equals_Object
    {
        [Fact]
        public void ReturnsFalseForNullObject()
        {
            var target = new InMemoryCosmosClient();

            Assert.False(target.Equals(null));
        }

        [Fact]
        public void ReturnsFalseForDifferentClient()
        {
            var target = new InMemoryCosmosClient();

            Assert.False(target.Equals(new InMemoryCosmosClient()));
        }

        [Fact]
        public void ReturnsTrueForSelf()
        {
            var target = new InMemoryCosmosClient();

            Assert.True(target.Equals(target));
        }
    }

    public class GetContainer_String_String
    {
        [Fact]
        public void ReturnsContainerProxy()
        {
            string databaseId = Any.String();
            string containerId = Any.String();

            var target = new InMemoryCosmosClient();

            var actual = target.GetContainer(databaseId, containerId);

            Assert.IsType<InMemoryContainerProxy>(actual);
        }

        [Fact]
        public void ReturnsRequestedContainer()
        {
            string databaseId = Any.String();
            string containerId = Any.String();

            var target = new InMemoryCosmosClient();

            var actual = target.GetContainer(databaseId, containerId);

            Assert.Equal(databaseId, actual.Database.Id);
            Assert.Equal(containerId, actual.Id);
        }
    }

    public class GetDatabase_String
    {
        [Fact]
        public void ReturnsDatabaseProxy()
        {
            string databaseId = Any.String();

            var target = new InMemoryCosmosClient();

            var actual = target.GetDatabase(databaseId);

            Assert.IsType<InMemoryDatabaseProxy>(actual);
        }

        [Fact]
        public void ReturnsRequestedDatabase()
        {
            string databaseId = Any.String();

            var target = new InMemoryCosmosClient();

            var actual = target.GetDatabase(databaseId);

            Assert.Equal(databaseId, actual.Id);
        }
    }

    public class GetDatabaseQueryIterator_1_QueryDefinition_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var target = new InMemoryCosmosClient();

            Assert.Throws<NotImplementedException>(
                () => target.GetDatabaseQueryIterator<object>(new QueryDefinition("x"), null, null));
        }
    }

    public class GetDatabaseQueryIterator_1_String_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var target = new InMemoryCosmosClient();

            Assert.Throws<NotImplementedException>(
                () => target.GetDatabaseQueryIterator<object>("x", null, null));
        }
    }

    public class GetDatabaseQueryStreamIterator_QueryDefinition_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var target = new InMemoryCosmosClient();

            Assert.Throws<NotImplementedException>(
                () => target.GetDatabaseQueryStreamIterator(new QueryDefinition("x"), null, null));
        }
    }

    public class GetDatabaseQueryStreamIterator_String_String_QueryRequestOptions
    {
        [Fact]
        public void ThrowsNotImplementedException()
        {
            var target = new InMemoryCosmosClient();

            Assert.Throws<NotImplementedException>(
                () => target.GetDatabaseQueryStreamIterator("x", null, null));
        }
    }

    public class GetHashCodeMethod
    {
        [Fact]
        public void ReturnsExpectedHashCode()
        {
            var target = new InMemoryCosmosClient();

            int expected = RuntimeHelpers.GetHashCode(target);

            Assert.Equal(expected, target.GetHashCode());
        }
    }

    public class ReadAccountAsync
    {
        [Fact]
        public async Task ThrowsNotImplementedException()
        {
            var target = new InMemoryCosmosClient();

            await Assert.ThrowsAsync<NotImplementedException>(target.ReadAccountAsync);
        }
    }

    public class ToStringMethod
    {
        [Fact]
        public void ReturnsTypeName()
        {
            var target = new InMemoryCosmosClient();

            Assert.Equal(typeof(InMemoryCosmosClient).FullName, target.ToString());
        }
    }

    public class Dispose
    {
        [Fact]
        public void DoesNotThrowException()
        {
            var target = new InMemoryCosmosClient();

            target.Dispose();
        }
    }

    [CollectionDefinition("InMemoryCosmosClient collection")]
    public class InMemoryCosmosClientCollection : ICollectionFixture<InMemoryCosmosClientFixture>
    {
    }

    public class InMemoryCosmosClientFixture
    {
    }
}
