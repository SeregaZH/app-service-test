using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ConfigurationService.Models;

namespace ConfigurationService.Services
{
    public interface IAttachmentsService
    {
        Task<IEnumerable<Attachment>> CreateAttachmentsAsync(Guid deviceId, IEnumerable<Attachment> proxy);

        Task<Stream> ReadAttachmentContentAsync(string name);
    }
}
