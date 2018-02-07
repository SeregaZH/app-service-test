using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConfigurationService.Models;
using ConfigurationService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConfigurationService.Controllers
{
    [Route("api/[controller]")]
    public class DevicesController : Controller
    {
        private readonly IDeviceService<Guid> _deviceService;
        private readonly IDeviceAttachmentsService _deviceAttachmentsService;

        public DevicesController(
            IDeviceService<Guid> deviceService, 
            IDeviceAttachmentsService deviceAttachmentsService
            )
        {
            _deviceService = deviceService;
            _deviceAttachmentsService = deviceAttachmentsService;
        }
        
        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<Device>> Get()
        {
            return await _deviceService.GetAllAsync();
        }

        // GET api/values/5
        [HttpGet("{id}", Name = "device")]
        public async Task<Device> Get(Guid id)
        {
            return await _deviceService.GetByIdAsync(id);
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Device device)
        {
            var entity = await _deviceService.CreateAsync(device);
            return CreatedAtRoute("device", new { id = entity.Id }, entity);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody]Device device)
        {
            var entity = await _deviceService.UpdateAsync(id, device);
            return Ok(entity);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id, [FromQuery]string eTag)
        {
            await _deviceService.DeleteAsync(id, eTag);
            return Ok();
        }

        [HttpGet("{id}/attachments/{attachmentName}", Name = "attachmentsGet")]
        public async Task<IActionResult> GetAttachment(Guid id, string attachmentName)
        {
            return new FileStreamResult(await _deviceAttachmentsService.ReadAttachmentContentAsync(id, attachmentName), "application/octet-stream");
        }

        [HttpPost("{id}/attachments")]
        public async Task<IActionResult> CreateAttachment(Guid id, IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            {
                return CreatedAtRoute("attachmentsGet", new { id, attachmentName = file.Name },
                    await _deviceAttachmentsService.CreateAttacmentAsync(id, file.Name, stream));
            }
        }
    }
}
