using System;
using System.Collections.Generic;

namespace ConfigurationService.Models
{
    public sealed class Device : DocumentEntity
    {
        public string Type { get; set; }
        public Configuration Config { get; set; }
        public List<Attachment> Instructions { get; set; }
    }
}
