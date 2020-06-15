using System.Collections.Generic;
using Sitecore.Commerce.Core;

namespace Kjaneb.Commerce.Plugin.Tax.Framework.Components
{
    public class NexusAddressComponent : Component
    {
        public IList<Party> Parties { get; set; }
    }
}
