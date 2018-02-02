using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ConfigurationService.Data
{
    public interface IFileRepository
    {
        Task<IEnumerable<string>> CreateBatchAsync(IEnumerable<FileInfo> fileInfos, FolderStructure folder);

        Task<string> CreateAsync(Stream fileStream, string fileName, FolderStructure folder);

        Task<Stream> GetContentAsync(string name, FolderStructure folder);
    }
}
