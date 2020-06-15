using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kjaneb.Commerce.Plugin.Tax.Framework.Models
{
    public class NexusAddress
    {
        public Guid Id { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
    }
}
