using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConfigurationService.Models;
using ConfigurationService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConfigurationService.Controllers
{
    [Route("api/[controller]")]
    public class DevicesController : Controller
    {
        private readonly IDeviceService<Guid> _deviceService;
        private readonly IAttachmentsService _attachmentsService;

        public DevicesController(IDeviceService<Guid> deviceService, IAttachmentsService attachmentsService)
        {
            _deviceService = deviceService;
            _attachmentsService = attachmentsService;
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
            throw new NotImplementedException();
            return Ok();
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            throw new NotImplementedException();
            return Ok();
        }

        [HttpGet("{id}/attachments/{attachmentName}")]
        public async Task<IActionResult> GetAttachment(Guid id, string attachmentName)
        {
            return new FileStreamResult(await _attachmentsService.ReadAttachmentContentAsync(attachmentName), "application/pdf");
        }
    }
}
