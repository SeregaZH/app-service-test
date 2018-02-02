using System.Threading.Tasks;
using ConfigurationService.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConfigurationService.Controllers
{
    [Route("api/[controller]")]
    public sealed class FilesController: Controller
    {
        private readonly IFileRepository _attachmentRepository;

        public FilesController(IFileRepository attachmentRepository)
        {
            _attachmentRepository = attachmentRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormFile file)
        {
            string fileName;
            using (var stream = file.OpenReadStream())
            {
                fileName = await _attachmentRepository.CreateAsync(stream, file.FileName, new FolderStructure(User?.Identity?.Name ?? "default"));
            }
            return Ok(new { fileName });
        }
    }
}
