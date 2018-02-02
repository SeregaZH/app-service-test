using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ConfigurationService.Data;
using ConfigurationService.Models;
using Microsoft.AspNetCore.Hosting;

namespace ConfigurationService.Services
{
    public sealed class DeviceAttachmentService : IDeviceAttachmentsService
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IFileRepository _fileRepository;
        private readonly IFolderStructureProvider _folderStructureProvider;
        
        public DeviceAttachmentService(
            IHostingEnvironment hostingEnvironment,
            IFileRepository fileRepository, 
            IFolderStructureProvider folderStructureProvider)
        {
            _hostingEnvironment = hostingEnvironment;
            _fileRepository = fileRepository;
            _folderStructureProvider = folderStructureProvider;
        }

        public async Task<IEnumerable<Attachment>> CreateInstructionsAsync(Guid deviceId, IEnumerable<Attachment> proxy)
        {
            var inputFiles = proxy.ToList();
            await _fileRepository.CreateBatchAsync(inputFiles.Select(CreateInstructionsInformation), _folderStructureProvider.CreateFolderStructure("devices", "instructions"));
            return inputFiles.Select(x => MapAttachments(x, deviceId));
        }

        public async Task<Stream> ReadInstructionContentAsync(string name)
        {
            return await _fileRepository.GetContentAsync(name, _folderStructureProvider.CreateFolderStructure("devices", "instructions"));
        }

        public async Task<Stream> ReadAttachmentContentAsync(Guid deviceId, string name)
        {
            var paramResolvers = new Dictionary<string, Func<dynamic, string>> { { nameof(deviceId), a => a.deviceId.ToString("N") } }.ToImmutableDictionary();
            var folderStructure =
                _folderStructureProvider.CreateFolderStructure("devices", "attachments", new { deviceId },
                    paramResolvers);
            return await _fileRepository.GetContentAsync(name, folderStructure);
        }

        public async Task<string> CreateAttacmentAsync(Guid deviceId, string name, Stream content)
        {
            var paramResolvers = new Dictionary<string, Func<dynamic, string>> {{ nameof(deviceId), a => a.deviceId.ToString("N") }}.ToImmutableDictionary(); 
            var folderStructure =
                _folderStructureProvider.CreateFolderStructure("devices", "attachments", new {deviceId},
                    paramResolvers);
            return await _fileRepository.CreateAsync(content, name, folderStructure);
        }

        private static Attachment MapAttachments(Attachment attachment, Guid deviceId)
        {
            return new Attachment
            {
                Name = attachment.Name,
                Title = attachment.Title,
                RelativePath = $"/api/devices/{deviceId.ToString()}/attachments/{attachment.Name}"
            };
        }

        private FileInfo CreateInstructionsInformation(Attachment attachment)
        {
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "instructions", attachment.Name);
            return new FileInfo(filePath);
        }
    }
}
