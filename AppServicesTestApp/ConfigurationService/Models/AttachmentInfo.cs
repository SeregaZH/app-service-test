using System.IO;

namespace ConfigurationService.Models
{
    public sealed class AttachmentInfo
    {
        public FileInfo FileInfo { get; set; }
        public string Title { get; set; }
    }
}
