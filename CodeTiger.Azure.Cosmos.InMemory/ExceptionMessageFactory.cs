using System.Diagnostics.CodeAnalysis;

namespace CodeTiger.Azure.Cosmos.InMemory;

/// <summary>
/// Creates error messages that are similar to error messages from the official Cosmos client.
/// </summary>
[SuppressMessage("CodeTiger.Layout", "CT3531:Lines should not exceed the maximum length of 115.",
    Justification = "The messages are much more easily managed without breaking up the long lines.")]
internal static class ExceptionMessageFactory
{
    internal static string CreateDatabaseExists(string activityId)
    {
        return @$"Response status code does not indicate success: Conflict (409); Substatus: 3206; ActivityId: {activityId}; Reason: (
code: Conflict
message : Entity with the specified id already exists in the system.
ActivityId: {activityId}
);";
    }

    internal static string CreateDatabaseNotFound(string activityId)
    {
        return $@"Response status code does not indicate success: NotFound (404); Substatus: 1003; ActivityId: {activityId}; Reason: (Message: {{""Errors"":[""Owner resource does not exist""]}});";
    }

    internal static string CreateContainerExists(string activityId)
    {
        return $@"Response status code does not indicate success: Conflict (409); Substatus: 0; ActivityId: {activityId}; Reason: (
code : Conflict
message : Message: {{""Errors"":[""Resource with specified id, name, or unique index already exists.""]}}
ActivityId: {activityId}
);";
    }

    internal static string CreateContainerNotFound(string activityId)
    {
        return $@"Response status code does not indicate success: Conflict (409); Substatus: 0; ActivityId: {activityId}; Reason: (
code : NotFound
message : Message: {{""Errors"":[""Resource Not Found. Learn more: https://aka.ms/cosmosdb-tsg-not-found""]}}
ActivityId: {activityId}
);";
    }

    internal static string CreatePartitionNotFound(string activityId)
    {
        return $@"Response status code does not indicate success: NotFound (404); Substatus: 0; ActivityId: {activityId}; Reason: (Message: {{""Errors"":[""Resource Not Found. Learn more: https://aka.ms/cosmosdb-tsg-not-found""]}});";
    }

    internal static string CreateItemNotFound(string activityId)
    {
        return $@"Response status code does not indicate success: NotFound (404); Substatus: 0; ActivityId: {activityId}; Reason: (Message: {{""Errors"":[""Resource Not Found. Learn more: https://aka.ms/cosmosdb-tsg-not-found""]}});";
    }
}
