using Microsoft.WindowsAzure.Storage.Blob;

namespace ConfigurationService.Data
{
    public interface IStorageClientProvider
    {
        CloudBlobClient CreateBlobStorage();
    }
}
