using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConfigurationService.Data;
using ConfigurationService.Models;
using Microsoft.AspNetCore.Mvc;

namespace ConfigurationService.Controllers
{
    [Route("api/[controller]")]
    public class DevicesController : Controller
    {
        private readonly IDocumentRepository<Device, Guid> _deviceRepository;

        public DevicesController(IDocumentRepository<Device, Guid> deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }
        
        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<Device>> Get()
        {
            return await _deviceRepository.GetAllAsync();
        }

        // GET api/values/5
        [HttpGet("{id}", Name = "device")]
        public async Task<Device> Get(Guid id)
        {
            return await _deviceRepository.GetByIdAsync(id);
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Device device)
        {
            var entity = await _deviceRepository.AddOrUpdateAsync(device);
            return CreatedAtRoute("device", new { id = entity.Id }, entity);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody]Device device)
        {
            var entity = await _deviceRepository.AddOrUpdateAsync(device);
            return Ok(entity);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _deviceRepository.RemoveAsync(id);
            return Ok();
        }
    }
}
