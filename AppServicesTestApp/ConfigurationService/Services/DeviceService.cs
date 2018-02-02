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
        private readonly IDeviceAttachmentsService _deviceAttachmentsService;
        
        public DeviceService(
            IDocumentRepository<Device, Guid> deviceRepository, 
            IDeviceAttachmentsService deviceAttachmentsService
        )
        { 
            _deviceRepository = deviceRepository;
            _deviceAttachmentsService = deviceAttachmentsService;
        }

        public async Task<Device> CreateAsync(Device device)
        {
            device.Instructions = (await _deviceAttachmentsService.CreateInstructionsAsync(device.DeviceId, device.Instructions)).ToList();
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
