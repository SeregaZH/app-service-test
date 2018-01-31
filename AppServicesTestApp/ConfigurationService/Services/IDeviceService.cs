﻿using System.Collections.Generic;
using System.Threading.Tasks;
using ConfigurationService.Models;

namespace ConfigurationService.Services
{
    public interface IDeviceService<in TId>
    {
        Task<Device> CreateAsync(Device device);
        Task<IEnumerable<Device>> GetAllAsync();

        Task<Device> GetByIdAsync(TId id);
    }
}
