using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ConfigurationService.Data;
using ConfigurationService.Models;
using ConfigurationService.Services.Exceptions;

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

        public async Task<ActionResult<bool, object>> DeleteAsync(Guid id)
        {
            var isDeleted = await _deviceRepository.RemoveAsync(id);
            return new ActionResult<bool, object>(isDeleted);
        }

        public async Task<ActionResult<Device, object>> UpdateAsync(Guid id, Device device)
        {
            throw new NotImplementedException();
        }

        public async Task<Device> GetByIdAsync(Guid id)
        {
            return await _deviceRepository.GetByIdAsync(id);
        }
    }
}
