using System;
using System.Net;
using Microsoft.Azure.Cosmos;

namespace CodeTiger.Azure.Cosmos.InMemory;

internal class InMemoryDatabaseResponse : DatabaseResponse
{
    public override string ActivityId { get; }

    public override Database Database { get; }

    public override CosmosDiagnostics Diagnostics => throw new NotImplementedException();

    public override string ETag { get; }

    public override Headers Headers { get; }

    public override double RequestCharge { get; }

    public override DatabaseProperties Resource { get; }

    public override HttpStatusCode StatusCode { get; }

    public InMemoryDatabaseResponse(InMemoryDatabase database, HttpStatusCode statusCode, double requestCharge)
    {
        ActivityId = Guid.NewGuid().ToString();
        Database = database;
        ETag = $"\\\"{Guid.NewGuid()}\"\\";
        Headers = new Headers();
        RequestCharge = requestCharge;
        Resource = new DatabaseProperties(database.Id);
        StatusCode = statusCode;
    }
}
