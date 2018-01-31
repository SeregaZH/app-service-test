using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ConfigurationService.Models;

namespace ConfigurationService.Data
{
    public interface IAttachmentRepository<TType>
    {
        Task<IEnumerable<Attachment>> CreateBatchAsync(Guid deviceId, IEnumerable<Attachment> batch);

        Task<Stream> GetContentAsync(string name);
    }
}
