using System;
using System.Net;
using CodeTiger.Azure.Cosmos.InMemory;
using Xunit;

namespace UnitTests.CodeTiger.Azure.Cosmos.InMemory;

public class InMemoryDatabaseResponseTests
{
    public class Constructor_InMemoryDatabase_HttpStatusCode_Double
    {
        [Fact]
        public void SetsActivityIdToGuid()
        {
            var database = CreateDatabase();

            var target = new InMemoryDatabaseResponse(database, Any.Enum<HttpStatusCode>(), Any.Double());

            Assert.True(Guid.TryParse(target.ActivityId, out _));
        }

        [Fact]
        public void SetsDatabaseToProvidedDatabase()
        {
            var database = CreateDatabase();

            var target = new InMemoryDatabaseResponse(database, Any.Enum<HttpStatusCode>(), Any.Double());

            Assert.Same(database, target.Database);
        }

        [Fact]
        public void SetsETagToGuidWithCorrectFormatting()
        {
            var database = CreateDatabase();

            var target = new InMemoryDatabaseResponse(database, Any.Enum<HttpStatusCode>(), Any.Double());

            Assert.Matches("\\\"[0-9a-z]{8}-[0-9a-z]{4}-[0-9a-z]{4}-[0-9a-z]{4}-[0-9a-z]{12}\\\"", target.ETag);
        }

        [Fact]
        public void SetsRequestChargeToProvidedRequestCharge()
        {
            var database = CreateDatabase();

            double expected = Any.Double();

            var target = new InMemoryDatabaseResponse(database, Any.Enum<HttpStatusCode>(), expected);

            Assert.Equal(expected, target.RequestCharge);
        }

        [Fact]
        public void SetsResourceToProperDatabaseProperties()
        {
            var database = CreateDatabase();

            var target = new InMemoryDatabaseResponse(database, Any.Enum<HttpStatusCode>(), Any.Double());

            Assert.Equal(database.Id, target.Resource.Id);
        }

        [Fact]
        public void SetsStatusCodeToProvidedStatusCode()
        {
            var database = CreateDatabase();

            var expected = Any.Enum<HttpStatusCode>();

            var target = new InMemoryDatabaseResponse(database, expected, Any.Double());

            Assert.Equal(expected, target.StatusCode);
        }

        private static InMemoryDatabase CreateDatabase()
        {
            return new InMemoryDatabase(new InMemoryCosmosClient(), Any.String());
        }
    }
}
