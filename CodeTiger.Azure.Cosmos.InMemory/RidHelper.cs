using System;
using System.Linq;

namespace CodeTiger.Azure.Cosmos.InMemory;

internal static class RidHelper
{
    public static string CreateRidString(int databaseRid, int containerRid, long itemRid)
    {
        byte[] ridBytes = new byte[16];

        BitConverter.GetBytes(databaseRid).CopyTo(ridBytes, 0);
        BitConverter.GetBytes(containerRid).CopyTo(ridBytes, 4);
        BitConverter.GetBytes(itemRid).Reverse().ToArray().CopyTo(ridBytes, 8);

        return Convert.ToBase64String(ridBytes);
    }

    public static string CreateSelfLink(int databaseRid, int containerRid, long itemRid)
    {
        byte[] ridBytes = new byte[16];

        BitConverter.GetBytes(databaseRid).CopyTo(ridBytes, 0);
        BitConverter.GetBytes(containerRid).CopyTo(ridBytes, 4);
        BitConverter.GetBytes(itemRid).Reverse().ToArray().CopyTo(ridBytes, 8);

        string databaseRidBase64 = Convert.ToBase64String(ridBytes, 0, 4);
        string collectionRidBase64 = Convert.ToBase64String(ridBytes, 0, 8);
        string documentRidBase64 = Convert.ToBase64String(ridBytes);

        return $"dbs/{databaseRidBase64}/colls/{collectionRidBase64}/docs/{documentRidBase64}/";
    }
}
