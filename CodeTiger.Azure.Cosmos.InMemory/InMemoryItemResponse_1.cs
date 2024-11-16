using System.Net;
using Microsoft.Azure.Cosmos;

namespace CodeTiger.Azure.Cosmos.InMemory;

internal class InMemoryItemResponse<T> : ItemResponse<T>
{
    public override HttpStatusCode StatusCode { get; }

    public override T Resource { get; }

    public InMemoryItemResponse(HttpStatusCode httpStatusCode, T item)
    {
        StatusCode = httpStatusCode;
        Resource = item;
    }
}
