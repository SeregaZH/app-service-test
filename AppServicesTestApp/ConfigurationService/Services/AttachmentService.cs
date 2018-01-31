using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ConfigurationService.Data;
using ConfigurationService.Models;
using Microsoft.AspNetCore.Hosting;

namespace ConfigurationService.Services
{
    public sealed class AttachmentService : IAttachmentsService
    {
        private readonly IAttachmentRepository<Device> _attachmentRepository; 

        public AttachmentService(
            IHostingEnvironment hostingEnvironment,
            IAttachmentRepository<Device> attachmentRepository)
        {
            _attachmentRepository = attachmentRepository;
        }

        public async Task<IEnumerable<Attachment>> CreateAttachmentsAsync(Guid deviceId, IEnumerable<Attachment> proxy)
        {
            return await _attachmentRepository.CreateBatchAsync(deviceId, proxy);
        }

        public async Task<Stream> ReadAttachmentContentAsync(string name)
        {
            return await _attachmentRepository.GetContentAsync(name);
        }
    }
}
