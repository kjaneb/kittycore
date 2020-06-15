using Kjaneb.Commerce.Plugin.Tax.Framework.Models;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Pipelines;

namespace Kjaneb.Commerce.Plugin.Tax.Framework.Pipelines
{
    public interface ICallExternalServiceToCalculateTaxPipeline : IPipeline<TaxableCartModel, TaxBreakdownModel, CommercePipelineExecutionContext>
    {
    }
}
