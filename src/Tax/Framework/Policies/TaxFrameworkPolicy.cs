using System.Collections.Generic;
using Kjaneb.Commerce.Plugin.Tax.Framework.Models;
using Sitecore.Commerce.Core;

namespace Kjaneb.Commerce.Plugin.Tax.Framework.Policies
{
    public class TaxFrameworkPolicy : Policy
    {
        public List<NexusAddress> NexusAddresses { get; set; }

        public List<string> FulfillmentDiscountActions { get; set; }
    }
}
