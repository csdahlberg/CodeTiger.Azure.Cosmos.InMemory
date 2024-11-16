using System.Configuration;
using AspNetCoreWebApiSample.Repositories;
using CodeTiger.Azure.Cosmos.InMemory;
using Microsoft.Azure.Cosmos;

namespace AspNetCoreWebApiSample;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddTransient<NoteRepository>();
        builder.Services.AddSingleton(provider =>
        {
            return new InMemoryCosmosClient();
            var configuration = provider.GetRequiredService<IConfiguration>();
            string? connectionString = configuration["ConnectionStrings:DefaultConnection"];
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ConfigurationErrorsException(
                    "The 'ConnectionStrings:DefaultConnection' configuration value is not set, but is required.");
            }

            return new CosmosClient(connectionString);
        });

        var app = builder.Build();
        app.MapControllers();
        app.Run();
    }
}
