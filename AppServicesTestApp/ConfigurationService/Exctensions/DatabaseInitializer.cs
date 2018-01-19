using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ConfigurationService.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ConfigurationService.Exctensions
{
    public static class DatabaseInitializer
    {
        private const string ConnectionStringName = "documentDBConnectionString";

        public static IServiceCollection AddDatabaseClient(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<IDocumentClient>(sl =>
            {
                var appConfig = sl.GetService< IConfiguration>();
                var connectionString = appConfig.GetConnectionString(ConnectionStringName);
                var authKey = appConfig.GetValue<string>("AuthKey");
                return new DocumentClient(new Uri(connectionString), authKey);
            });
            return serviceCollection;
        }

        public static async Task<IApplicationBuilder> InitializeDatabaseAsync(this IApplicationBuilder app)
        {
            var client = (IDocumentClient)app.ApplicationServices.GetService(typeof(IDocumentClient));
            var appConfig = (IConfiguration)app.ApplicationServices.GetService(typeof(IConfiguration));
            var databaseId = appConfig.GetValue<string>("DatabaseId");
            await client.CreateDatabaseIfNotExistsAsync(databaseId);
            await client.CreateCollectionIfNotExistsAsync<Device>(databaseId, null, null, new PartitionKeyDefinition { Paths = new Collection<string>(new[] { "/DeviceId" })} );
            return app;
        }
    }
}
