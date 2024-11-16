using System;
using System.Net;
using Microsoft.Azure.Cosmos;
using UnitTests.CodeTiger.Azure.Cosmos.InMemory;
using Xunit;

namespace CodeTiger.Azure.Cosmos.InMemory;

public class InMemoryContainerResponseTests
{
    public class Constructor_InMemoryContainer_HttpStatusCode_Double
    {
        [Fact]
        public void SetsActivityIdToGuid()
        {
            var target = new InMemoryContainerResponse(
                new InMemoryContainer(Any.Int32(), new InMemoryDatabase(new InMemoryCosmosClient(), Any.String()),
                    new ContainerProperties(Any.String(), "/partitionKey")),
                Any.Enum<HttpStatusCode>(),
                Any.Double());

            Assert.True(Guid.TryParse(target.ActivityId, out _));
        }

        [Fact]
        public void SetsContainerToProvidedTestContainer()
        {
            var target = new InMemoryContainerResponse(
                new InMemoryContainer(Any.Int32(), new InMemoryDatabase(new InMemoryCosmosClient(), Any.String()),
                    new ContainerProperties(Any.String(), "/partitionKey")),
                Any.Enum<HttpStatusCode>(),
                Any.Double());

            Assert.True(Guid.TryParse(target.ActivityId, out _));
        }

        [Fact]
        public void SetsETagToCorrectlyFormattedGuid()
        {
            var target = new InMemoryContainerResponse(
                new InMemoryContainer(Any.Int32(), new InMemoryDatabase(new InMemoryCosmosClient(), Any.String()),
                    new ContainerProperties(Any.String(), "/partitionKey")),
                Any.Enum<HttpStatusCode>(),
                Any.Double());

            Assert.Matches("\\\"[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}\\\"", target.ETag);
        }

        [Fact]
        public void SetsHeadersToEmptyHeaderCollection()
        {
            var target = new InMemoryContainerResponse(
                new InMemoryContainer(Any.Int32(), new InMemoryDatabase(new InMemoryCosmosClient(), Any.String()),
                    new ContainerProperties(Any.String(), "/partitionKey")),
                Any.Enum<HttpStatusCode>(),
                Any.Double());

            Assert.Empty(target.Headers);
        }

        [Fact]
        public void SetsRequestChargeToProvidedValue()
        {
            double requestCharge = Any.Double();

            var target = new InMemoryContainerResponse(
                new InMemoryContainer(Any.Int32(), new InMemoryDatabase(new InMemoryCosmosClient(), Any.String()),
                    new ContainerProperties(Any.String(), "/partitionKey")),
                Any.Enum<HttpStatusCode>(),
                requestCharge);

            Assert.Equal(requestCharge, target.RequestCharge);
        }

        [Fact]
        public void SetsResourceToCorrectContainerProperties()
        {
            string containerId = Any.String();
            string partitionKey = "/" + Any.String();

            var target = new InMemoryContainerResponse(
                new InMemoryContainer(Any.Int32(), new InMemoryDatabase(new InMemoryCosmosClient(), Any.String()),
                    new ContainerProperties(containerId, partitionKey)),
                Any.Enum<HttpStatusCode>(),
                Any.Double());

            Assert.Equal(containerId, target.Resource.Id);
            Assert.Equal(partitionKey, target.Resource.PartitionKeyPath);
        }

        [Fact]
        public void SetsStatusCodeToProvidedStatusCode()
        {
            var expected = Any.Enum<HttpStatusCode>();

            var target = new InMemoryContainerResponse(
                new InMemoryContainer(Any.Int32(), new InMemoryDatabase(new InMemoryCosmosClient(), Any.String()),
                    new ContainerProperties(Any.String(), "/partitionKey")),
                expected,
                Any.Double());

            Assert.Equal(expected, target.StatusCode);
        }
    }
}
