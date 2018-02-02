using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ConfigurationService.Models;

namespace ConfigurationService.Services
{
    public interface IDeviceAttachmentsService
    {
        Task<IEnumerable<Attachment>> CreateInstructionsAsync(Guid deviceId, IEnumerable<Attachment> proxy);
        
        Task<Stream> ReadInstructionContentAsync(string name);

        Task<Stream> ReadAttachmentContentAsync(Guid deviceId, string name);

        Task<string> CreateAttacmentAsync(Guid deviceId, string name, Stream content);
    }
}
