using System.Collections.Generic;
using System.Threading.Tasks;
using ConfigurationService.Models;

namespace ConfigurationService.Services
{
    public interface IDeviceService<in TId>
    {
        Task<Device> CreateAsync(Device device);
        Task<IEnumerable<Device>> GetAllAsync();

        Task<ActionResult<bool, object>> DeleteAsync(TId id);

        Task<ActionResult<Device, object>> UpdateAsync(TId id, Device device);

        Task<Device> GetByIdAsync(TId id);
    }
}
