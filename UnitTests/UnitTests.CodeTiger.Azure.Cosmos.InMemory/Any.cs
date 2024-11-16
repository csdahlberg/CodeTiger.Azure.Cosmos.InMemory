using System;

namespace UnitTests.CodeTiger.Azure.Cosmos.InMemory;

internal static class Any
{
    private static readonly Random _random = new Random();

    public static string String()
    {
        return Guid.NewGuid().ToString();
    }

    public static string PartitionKey()
    {
        return "/" + Guid.NewGuid().ToString().Replace('-', '_');
    }

    public static bool Boolean()
    {
        return _random.Next() % 2 == 0;
    }

    public static int Int32()
    {
        return _random.Next();
    }

    public static long Int64()
    {
        return _random.NextInt64();
    }

    public static double Double()
    {
        return _random.NextDouble();
    }

    public static T Enum<T>()
        where T : struct, Enum
    {
        var enumValues = System.Enum.GetValues<T>();

        return enumValues[_random.Next(0, enumValues.Length - 1)];
    }
}
