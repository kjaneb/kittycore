using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kjaneb.Commerce.Plugin.Tax.Framework.Models;
using Kjaneb.Commerce.Plugin.Tax.Framework.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Fulfillment;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;

namespace Kjaneb.Commerce.Plugin.Tax.Framework.Pipelines.Blocks
{
    public class GetNexusAddressFromPolicyBlock : PipelineBlock<Cart, IEnumerable<NexusAddress>, CommercePipelineExecutionContext>
    {
        public override Task<IEnumerable<NexusAddress>> Run(Cart arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull(Name + ": The cart can not be null");
            if (!arg.HasComponent<FulfillmentComponent>())
                return Task.FromResult<IEnumerable<NexusAddress>>(null);
            var policy = context.GetPolicy<TaxFrameworkPolicy>();
            return Task.FromResult(policy.NexusAddresses.AsEnumerable());
        }
    }
}
