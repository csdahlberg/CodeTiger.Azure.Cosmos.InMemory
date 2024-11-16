using System;
using CodeTiger.Azure.Cosmos.InMemory;
using Xunit;

namespace UnitTests.CodeTiger.Azure.Cosmos.InMemory;

public static class ExceptionMessageFactoryTests
{
    public static class CreateContainerExists_String
    {
        [Fact]
        public static void ReturnsMessageWithActivityId()
        {
            string activityId = Guid.NewGuid().ToString();

            string actual = ExceptionMessageFactory.CreateContainerExists(activityId);

            Assert.Contains($"ActivityId: {activityId}", actual);
        }
    }

    public static class CreateContainerNotFound_String
    {
        [Fact]
        public static void ReturnMessageWithActivityId()
        {
            string activityId = Guid.NewGuid().ToString();

            string actual = ExceptionMessageFactory.CreateContainerNotFound(activityId);

            Assert.Contains($"ActivityId: {activityId}", actual);
        }
    }

    public static class CreateDatabaseNotFound_String
    {
        [Fact]
        public static void ReturnsMessageWithActivityId()
        {
            string activityId = Guid.NewGuid().ToString();

            string actual = ExceptionMessageFactory.CreateDatabaseNotFound(activityId);

            Assert.Contains($"ActivityId: {activityId}", actual);
        }
    }

    public static class CreateDatabaseExists_String
    {
        [Fact]
        public static void ReturnMessageWithActivityId()
        {
            string activityId = Guid.NewGuid().ToString();

            string actual = ExceptionMessageFactory.CreateDatabaseExists(activityId);

            Assert.Contains($"ActivityId: {activityId}", actual);
        }
    }
}
