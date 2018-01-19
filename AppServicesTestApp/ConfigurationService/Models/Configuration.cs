using System.Collections.Generic;

namespace ConfigurationService.Models
{
    public sealed class Configuration
    {
        public string FirmwareVersion { get; set; }
        public List<Pin> Pins { get; set; }
    }
}