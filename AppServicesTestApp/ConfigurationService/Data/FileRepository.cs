using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ConfigurationService.Data
{
    public sealed class FileRepository: IFileRepository
    {
        private readonly IStorageClientProvider _clientProvider;

        public FileRepository(IStorageClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        public async Task<IEnumerable<string>> CreateBatchAsync(IEnumerable<FileInfo> fileInfos, FolderStructure folder)
        {
            var client = _clientProvider.CreateBlobStorage();
            var container = client.GetContainerReference(folder.ContainerName);
            await container.CreateIfNotExistsAsync();
            await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            var queries = fileInfos
                .Where(x => !string.IsNullOrEmpty(x.Name))
                .Select(async attachmentInfo =>
                {
                    var blockBlob = container.GetBlockBlobReference(attachmentInfo.Name);

                    if (await blockBlob.ExistsAsync())
                    {
                        return attachmentInfo.Name;
                    }

                    using (var fileStream = attachmentInfo.OpenRead())
                    {
                        await blockBlob.UploadFromStreamAsync(fileStream);
                        return attachmentInfo.Name;
                    }
                });

            var result = await Task.WhenAll(queries);
            return result;
        }

        public async Task<string> CreateAsync(Stream fileStream, string fileName, FolderStructure folder)
        {
            var client = _clientProvider.CreateBlobStorage();
            var blobContainer = client.GetContainerReference(folder.ContainerName);
            await blobContainer.CreateIfNotExistsAsync();
            var directory = blobContainer.GetDirectoryReference(folder.RelativePath);
            var blockBlob = directory.GetBlockBlobReference(fileName);
            await blockBlob.UploadFromStreamAsync(fileStream);
            return fileName;
        }

        public async Task<Stream> GetContentAsync(string name, FolderStructure folder)
        {
            var client = _clientProvider.CreateBlobStorage();
            var container = client.GetContainerReference(folder.ContainerName);
            var directory = container.GetDirectoryReference(folder.RelativePath);
            var blockBlob = directory.GetBlobReference(name);
            return await blockBlob.OpenReadAsync();
        }
    }
}
