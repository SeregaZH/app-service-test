using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ConfigurationService.Data;
using ConfigurationService.Models;

namespace ConfigurationService.Services
{
    public sealed class DeviceService : IDeviceService<Guid>
    {
        private readonly IDocumentRepository<Device, Guid> _deviceRepository;
        private readonly IAttachmentsService _attachmentsService;
        
        public DeviceService(
            IDocumentRepository<Device, Guid> deviceRepository, 
            IAttachmentsService attachmentsService
        )
        { 
            _deviceRepository = deviceRepository;
            _attachmentsService = attachmentsService;
        }

        public async Task<Device> CreateAsync(Device device)
        {
            device.Instructions = (await _attachmentsService.CreateAttachmentsAsync(device.DeviceId, device.Instructions)).ToList();
            return await _deviceRepository.AddOrUpdateAsync(device);
        }

        public async Task<IEnumerable<Device>> GetAllAsync()
        {
            return await _deviceRepository.GetAllAsync();
        }

        public async Task<Device> GetByIdAsync(Guid id)
        {
            return await _deviceRepository.GetByIdAsync(id);
        }
    }
}
