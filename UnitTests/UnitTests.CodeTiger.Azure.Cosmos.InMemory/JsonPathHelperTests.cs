using System;
using CodeTiger.Azure.Cosmos.InMemory;
using Xunit;

namespace UnitTests.CodeTiger.Azure.Cosmos.InMemory;

public class JsonPathHelperTests
{
    public class CreateFromXPath_String
    {
        [Theory]
        [InlineData("")]
        [InlineData("partitionKey")]
        [InlineData("\\partitionKey")]
        [InlineData("data/partitionKey")]
        public void ThrowsArgumentOutOfRangeExceptionWhenPathDoesNotStartWithAForwardSlash(string xPath)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => JsonPathHelper.CreateFromXPath(xPath));
        }

        [Theory]
        [InlineData("/partitionKey/")]
        [InlineData("/partitionKey//")]
        [InlineData("/data/partitionKey/")]
        public void ThrowsArgumentOutOfRangeExceptionWhenPathEndsWithAForwardSlash(string xPath)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => JsonPathHelper.CreateFromXPath(xPath));
        }

        [Theory]
        [InlineData("/*")]
        [InlineData("/*/partitionKey")]
        [InlineData("/partitionKey/*")]
        [InlineData("/?")]
        [InlineData("/?/partitionKey")]
        [InlineData("/partitionKey/?")]
        public void ThrowsArgumentOutOfRangeExceptionWhenPathContainsWildcards(string xPath)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => JsonPathHelper.CreateFromXPath(xPath));
        }

        [Theory]
        [InlineData("/partitionKey", "$.partitionKey")]
        [InlineData("/partition_key", "$.partition_key")]
        [InlineData("/data/partitionKey", "$.data.partitionKey")]
        public void ReturnsCorrectJsonPathsForValidXPaths(string xPath, string jsonPath)
        {
            string actual = JsonPathHelper.CreateFromXPath(xPath);

            Assert.Equal(jsonPath, actual);
        }
    }
}
