using System;
using System.Net;
using Microsoft.Azure.Cosmos;

namespace CodeTiger.Azure.Cosmos.InMemory;

internal class InMemoryContainerResponse : ContainerResponse
{
    private readonly InMemoryContainer _inMemoryContainer;

    public override string ActivityId { get; }

    public override Container Container => _inMemoryContainer;

    public override CosmosDiagnostics Diagnostics => throw new NotImplementedException();

    public override string ETag { get; }

    public override Headers Headers { get; }

    public override double RequestCharge { get; }

    public override ContainerProperties Resource { get; }

    public override HttpStatusCode StatusCode { get; }

    public InMemoryContainerResponse(InMemoryContainer inMemoryContainer, HttpStatusCode statusCode,
        double requestCharge)
    {
        ActivityId = Guid.NewGuid().ToString();
        _inMemoryContainer = inMemoryContainer;
        ETag = $"\\\"{Guid.NewGuid()}\"\\";
        Headers = new Headers();
        RequestCharge = requestCharge;
        Resource = new ContainerProperties(inMemoryContainer.Id, inMemoryContainer.Properties.PartitionKeyPath);
        StatusCode = statusCode;
    }
}
