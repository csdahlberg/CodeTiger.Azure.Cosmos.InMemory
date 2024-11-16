using System.IO;
using System.Text;
using CodeTiger.Azure.Cosmos.InMemory;
using Microsoft.Azure.Cosmos;
using Xunit;

namespace UnitTests.CodeTiger.Azure.Cosmos.InMemory;

public class InMemoryCosmosJsonDotNetSerializerTests
{
    public class FromStream_1_Stream
    {
        [Fact]
        public void ReturnsProvidedStreamWhenTypeArgumentIsStream()
        {
            var expected = new MemoryStream();

            var target = new InMemoryCosmosJsonDotNetSerializer();

            var actual = target.FromStream<Stream>(expected);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void ReturnsProvidedStreamWhenTypeArgumentIsASubclassOfStream()
        {
            var expected = new MemoryStream();

            var target = new InMemoryCosmosJsonDotNetSerializer();

            var actual = target.FromStream<MemoryStream>(expected);

            Assert.Same(expected, actual);
        }

        [Fact]
        public void ThrowsInvalidDataExceptionWhenStreamIsEmpty()
        {
            var stream = new MemoryStream();

            var target = new InMemoryCosmosJsonDotNetSerializer();

            Assert.Throws<InvalidDataException>(() => target.FromStream<Thing>(stream));
        }

        [Fact]
        public void DeserializesStreamWhenOptionsAreNotProvided()
        {
            string name = Any.String();

            string input = $$"""
                {
                  "Name": "{{name}}"
                }
                """;

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));

            var target = new InMemoryCosmosJsonDotNetSerializer();

            var actual = target.FromStream<Thing>(stream);

            Assert.Equal(name, actual.Name);
        }

        [Fact]
        public void DeserializesStreamWhenOptionsAreProvided()
        {
            string name = Any.String();

            string input = $$"""
                {
                  "name": "{{name}}"
                }
                """;

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(input));

            var target = new InMemoryCosmosJsonDotNetSerializer(new CosmosSerializationOptions
            {
                PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
            });

            var actual = target.FromStream<Thing>(stream);

            Assert.Equal(name, actual.Name);
        }
    }

    public class ToStream_1_T
    {
        [Fact]
        public void DoesNotWriteAnyBytesToStreamWhenObjectIsNull()
        {
            var target = new InMemoryCosmosJsonDotNetSerializer();

            byte[] actual;

            using (var stream = new MemoryStream())
            {
                target.ToStream<Thing?>(null);

                actual = stream.ToArray();
            }

            Assert.Empty(actual);
        }

        [Fact]
        public void SerializesUsingDefaultValuesWhenOptionsAreNotProvided()
        {
            string name = Any.String();

            byte[] actual;

            var target = new InMemoryCosmosJsonDotNetSerializer();

            using (var stream = target.ToStream(new Thing { Name = name }))
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                actual = memoryStream.ToArray();
            }

            string expected = $$"""{"Name":"{{name}}"}""";

            Assert.Equal(expected, Encoding.UTF8.GetString(actual));
        }

        [Fact]
        public void SerializesWithoutNullValuesWhenSpecified()
        {
            byte[] actual;

            var target = new InMemoryCosmosJsonDotNetSerializer(
                new CosmosSerializationOptions { IgnoreNullValues = true });

            using (var stream = target.ToStream(new Thing { Name = null }))
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                actual = memoryStream.ToArray();
            }

            string expected = "{}";

            Assert.Equal(expected, Encoding.UTF8.GetString(actual));
        }

        [Fact]
        public void SerializesWithIndentationWhenSpecified()
        {
            string name = Any.String();

            byte[] actual;

            var target = new InMemoryCosmosJsonDotNetSerializer(
                new CosmosSerializationOptions { Indented = true });

            using (var stream = target.ToStream(new Thing { Name = name }))
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                actual = memoryStream.ToArray();
            }

            string expected = $$"""
                {
                  "Name": "{{name}}"
                }
                """;

            Assert.Equal(expected, Encoding.UTF8.GetString(actual));
        }

        [Fact]
        public void SerializesWithCamelCasingWhenSpecified()
        {
            string name = Any.String();

            byte[] actual;

            var target = new InMemoryCosmosJsonDotNetSerializer(
                new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase,
                });

            using (var stream = target.ToStream(new Thing { Name = name }))
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                actual = memoryStream.ToArray();
            }

            string expected = $$"""{"name":"{{name}}"}""";

            Assert.Equal(expected, Encoding.UTF8.GetString(actual));
        }
    }

    private class Thing
    {
        public string? Name { get; set; }
    }
}
