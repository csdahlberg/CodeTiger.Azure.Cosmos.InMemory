using System;
using System.Linq;
using CodeTiger.Azure.Cosmos.InMemory;
using Xunit;

namespace UnitTests.CodeTiger.Azure.Cosmos.InMemory;

public class RidHelperTests
{
    public class CreateRidString_Int32_Int32_Int64
    {
        [Fact]
        public void ReturnsCorrectStringForRandomRids()
        {
            int databaseRid = Any.Int32();
            int containerRid = Any.Int32();
            long itemRid = Any.Int64();

            byte[] ridBytes = new byte[16];

            BitConverter.GetBytes(databaseRid).CopyTo(ridBytes, 0);
            BitConverter.GetBytes(containerRid).CopyTo(ridBytes, 4);
            BitConverter.GetBytes(itemRid).Reverse().ToArray().CopyTo(ridBytes, 8);

            string expected = Convert.ToBase64String(ridBytes);

            string actual = RidHelper.CreateRidString(databaseRid, containerRid, itemRid);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 1, 1, "AQAAAAEAAAAAAAAAAAAAAQ==")]
        [InlineData(0x01000000, 0x02000000, 0x03000000, "AAAAAQAAAAIAAAAAAwAAAA==")]
        public void ReturnsCorrectStringForSpecificRids(int databaseRid, int containerRid, long itemRid,
            string expected)
        {
            string actual = RidHelper.CreateRidString(databaseRid, containerRid, itemRid);

            Assert.Equal(expected, actual);
        }
    }

    public class CreateSelfLink_Int32_Int32_Int64
    {
        [Fact]
        public void ReturnsCorrectSelfLinkForRandomRids()
        {
            int databaseRid = Any.Int32();
            int containerRid = Any.Int32();
            long itemRid = Any.Int64();

            byte[] ridBytes = new byte[16];
            BitConverter.GetBytes(databaseRid).CopyTo(ridBytes, 0);
            BitConverter.GetBytes(containerRid).CopyTo(ridBytes, 4);
            BitConverter.GetBytes(itemRid).Reverse().ToArray().CopyTo(ridBytes, 8);

            string databaseRidBase64 = Convert.ToBase64String(ridBytes, 0, 4);
            string collectionRidBase64 = Convert.ToBase64String(ridBytes, 0, 8);
            string documentRidBase64 = Convert.ToBase64String(ridBytes);

            string expected = $"dbs/{databaseRidBase64}/colls/{collectionRidBase64}/docs/{documentRidBase64}/";

            string actual = RidHelper.CreateSelfLink(databaseRid, containerRid, itemRid);

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 1, 1, "dbs/AQAAAA==/colls/AQAAAAEAAAA=/docs/AQAAAAEAAAAAAAAAAAAAAQ==/")]
        [InlineData(0x01000000, 0x02000000, 0x03000000,
            "dbs/AAAAAQ==/colls/AAAAAQAAAAI=/docs/AAAAAQAAAAIAAAAAAwAAAA==/")]
        public void ReturnsCorrectSelfLinkForSpecificRids(int databaseRid, int containerRid, long itemRid,
            string expected)
        {
            string actual = RidHelper.CreateSelfLink(databaseRid, containerRid, itemRid);

            Assert.Equal(expected, actual);
        }
    }
}
