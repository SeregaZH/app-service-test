using System;

namespace ConfigurationService.Models
{
    public sealed class Device
    {
        public Guid Id { get; set; }
        public Guid DeviceId { get; set; }
        public string Type { get; set; }
        public Configuration Config { get; set; }
    }
}
