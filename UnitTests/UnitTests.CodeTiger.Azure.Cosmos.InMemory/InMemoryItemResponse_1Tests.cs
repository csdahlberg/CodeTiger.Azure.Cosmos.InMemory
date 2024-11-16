using System.Net;
using CodeTiger.Azure.Cosmos.InMemory;
using Xunit;

namespace UnitTests.CodeTiger.Azure.Cosmos.InMemory;

public class InMemoryItemResponse_1Tests
{
    public class Constructor_1_HttpStatusCode_T
    {
        [Fact]
        public void SetsStatusCodeToProvidedStatusCode()
        {
            var expected = Any.Enum<HttpStatusCode>();

            var target = new InMemoryItemResponse<string>(expected, Any.String());

            Assert.Equal(expected, target.StatusCode);
        }

        [Fact]
        public void SetsResourceToProvidedResource()
        {
            string expected = Any.String();

            var target = new InMemoryItemResponse<string>(Any.Enum<HttpStatusCode>(), expected);

            Assert.Equal(expected, target.Resource);
        }
    }
}
