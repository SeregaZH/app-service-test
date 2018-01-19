using System.Collections.Generic;

namespace ConfigurationService.Models
{
    public sealed class Pin
    {
        public int PhysicalNumber { get; set; }
        public string PrimaryName { get; set; }
        public string Description { get; set; }
        public List<Pin> Alt { get; set; }
    }
}
