using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kjaneb.Commerce.Plugin.Tax.Framework.Models;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Pipelines;

namespace Kjaneb.Commerce.Plugin.Tax.Framework.Pipelines
{
    public class CallExternalServiceToCalculateTaxPipeline : CommercePipeline<TaxableCartModel, TaxBreakdownModel>, ICallExternalServiceToCalculateTaxPipeline
    {
        public CallExternalServiceToCalculateTaxPipeline(IPipelineConfiguration<ICallExternalServiceToCalculateTaxPipeline> configuration, ILoggerFactory loggerFactory) : base(configuration, loggerFactory)
        {
        }
    }
}
