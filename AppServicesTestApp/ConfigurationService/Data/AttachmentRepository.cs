using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ConfigurationService.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ConfigurationService.Data
{
    public sealed class AttachmentRepository<TType>: IAttachmentRepository<TType>
    {
        private readonly CloudBlobClient _blobClient;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly string _containerName;

        public AttachmentRepository(IConfiguration configuration, IHostingEnvironment hostingEnvironment, string containerName)
        {
            _hostingEnvironment = hostingEnvironment;
            _containerName = containerName;
            var connection = configuration.GetConnectionString("BlobStore");
            var account = CloudStorageAccount.Parse(connection);
            _blobClient = account.CreateCloudBlobClient();
            _blobClient.DefaultRequestOptions = new BlobRequestOptions { ParallelOperationThreadCount = 6 };
        }

        public async Task<IEnumerable<Attachment>> CreateBatchAsync(Guid deviceId, IEnumerable<Attachment> batch)
        {
            var container = _blobClient.GetContainerReference(_containerName);
            await container.CreateIfNotExistsAsync();
            await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            var queries = batch.Select(async x =>
            {
                var blockBlob = container.GetBlockBlobReference(x.Name);

                if (await blockBlob.ExistsAsync())
                {
                    return new Attachment { Name = x.Name, Title = $"Instruction {x.Name}", RelativePath = $"/api/devices/{deviceId.ToString()}/attachments/{x.Name}" };
                }

                using (var fileStream = File.OpenRead(Path.Combine(_hostingEnvironment.WebRootPath, _containerName, $"{x.Name}.pdf")))
                {
                    await blockBlob.UploadFromStreamAsync(fileStream);
                    return new Attachment { Name = x.Name, Title = $"Instruction {x.Name}", RelativePath = $"/api/devices/{deviceId.ToString()}/attachments/{x.Name}" };
                }
            });

            var result = await Task.WhenAll(queries);
            return result;
        }

        public async Task<Stream> GetContentAsync(string name)
        {
            var container = _blobClient.GetContainerReference("instructions");
            var blockBlob = container.GetBlobReference(name);
            return await blockBlob.OpenReadAsync();
        }
    }
}
