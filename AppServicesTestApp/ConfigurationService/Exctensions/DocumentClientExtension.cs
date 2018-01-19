using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace ConfigurationService.Exctensions
{
    public static class DocumentClientExtension
    {
        public static async Task CreateDatabaseIfNotExistsAsync(this IDocumentClient client, string databaseId)
        {
            try
            {
                await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDatabaseAsync(new Database { Id = databaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        public static async Task CreateCollectionIfNotExistsAsync<T>(this IDocumentClient client, string databaseId, string collectionId = null, RequestOptions options = null, PartitionKeyDefinition keyDefinition = null)
        {
            var collectionName = collectionId ?? typeof(T).Name;
            try
            {
                await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseId, collectionName));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(databaseId),
                        new DocumentCollection { Id = collectionName, PartitionKey = keyDefinition ?? new PartitionKeyDefinition { Paths = new Collection<string>(new [] { "id" }) }}, 
                        options ?? new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
