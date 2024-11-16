using System;
using System.Linq;

namespace CodeTiger.Azure.Cosmos.InMemory;

internal static class JsonPathHelper
{
    private static readonly char[] _forwardSlashChar = new char[] { '/' };

    public static string CreateFromXPath(string xPath)
    {
        if (!xPath.StartsWith("/"))
        {
            throw new ArgumentOutOfRangeException(nameof(xPath), xPath,
                "Partition keys must begin with a '/' character.");
        }

        if (xPath.Any(x => x == '*' || x == '?'))
        {
            throw new ArgumentOutOfRangeException(nameof(xPath), xPath,
                "Partition keys must not contain wildcard characters.");
        }

        string[] propertyNames = xPath.Split(_forwardSlashChar, StringSplitOptions.RemoveEmptyEntries);

        if (!propertyNames.Any())
        {
            throw new ArgumentOutOfRangeException(nameof(xPath), xPath,
                "Partition keys must contain at least one property name.");
        }

        if (xPath.EndsWith("/"))
        {
            throw new ArgumentOutOfRangeException(nameof(xPath), xPath,
                "Partition keys must not end with a '/' character.");
        }

        return "$." + string.Join(".", propertyNames);
    }
}
