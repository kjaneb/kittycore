using System.Collections.Generic;
using Kjaneb.Commerce.Plugin.Tax.Framework.Models;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Pipelines;

namespace Kjaneb.Commerce.Plugin.Tax.Framework.Pipelines
{
    public class PopulateNexusAddressPipeline : CommercePipeline<Cart, IEnumerable<NexusAddress>>, IPopulateNexusAddressPipeline
    {
        public PopulateNexusAddressPipeline(IPipelineConfiguration<IPopulateNexusAddressPipeline> configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
        }
    }
}
