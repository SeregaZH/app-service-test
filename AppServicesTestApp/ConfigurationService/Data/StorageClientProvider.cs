using System;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace ConfigurationService.Data
{
    public sealed class StorageClientProvider: IStorageClientProvider
    {
        private readonly Lazy<CloudBlobClient> _blobClientInitializer;

        public StorageClientProvider(IConfiguration configuration)
        {
            _blobClientInitializer = new Lazy<CloudBlobClient>(() => InitializeBlobClient(configuration));
        }

        public CloudBlobClient CreateBlobStorage()
        {
            return _blobClientInitializer.Value;
        }

        private static CloudBlobClient InitializeBlobClient(IConfiguration configuration)
        {
            var connection = configuration.GetConnectionString("BlobStore");
            var account = CloudStorageAccount.Parse(connection);
            var client = account.CreateCloudBlobClient();
            client.DefaultRequestOptions = new BlobRequestOptions
            {
                RetryPolicy = new ExponentialRetry(),
                ParallelOperationThreadCount = 4
            };
            return client;
        }
    }
}
