using System;
using System.Collections.Generic;
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
            device.Instructions = (await _deviceAttachmentsService.CreateInstructionsAsync(device.Id, device.Instructions)).ToList();
            return await _deviceRepository.CreateAsync(device);
        }

        public async Task<IEnumerable<Device>> GetAllAsync()
        {
            return await _deviceRepository.GetAllAsync();
        }

        public async Task<ActionResult<Device, object>> DeleteAsync(Guid id, string eTag)
        {
            var deletedResource = await _deviceRepository.RemoveAsync(id, eTag);
            return new ActionResult<Device, object>(deletedResource);
        }

        public async Task<ActionResult<Device, object>> UpdateAsync(Guid id, Device device)
        {
            return new ActionResult<Device, object>(await _deviceRepository.UpdateAsync(id, device));
        }

        public async Task<Device> GetByIdAsync(Guid id)
        {
            return await _deviceRepository.GetByIdAsync(id);
        }
    }
}
