using System.Reflection;
using Kjaneb.Commerce.Plugin.Tax.Framework.Pipelines;
using Kjaneb.Commerce.Plugin.Tax.Framework.Pipelines.Blocks;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Tax;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines.Definitions.Extensions;

namespace Kjaneb.Commerce.Plugin.Tax.Framework
{
    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config
                .AddPipeline<IPopulateNexusAddressPipeline, PopulateNexusAddressPipeline>(
                    configure => { configure.Add<GetNexusAddressFromPolicyBlock>(); }
                )
                .AddPipeline<ICallExternalServiceToCalculateTaxPipeline, CallExternalServiceToCalculateTaxPipeline>()
                .ConfigurePipeline<ICalculateCartLinesPipeline>(builder => builder
                        .Remove<CalculateCartLinesTaxBlock>()
                    , "main", 5000)
                .ConfigurePipeline<ICalculateCartPipeline>(builder => builder
                        .Remove<Sitecore.Commerce.Plugin.Tax.CalculateCartTaxBlock>()
                        .Add<Pipelines.Blocks.CalculateCartTaxBlock>().Before<CalculateCartTotalsBlock>()
                    , "main", 5000)
                .ConfigurePipeline<IRunningPluginsPipeline>(c => { c.Add<Tax.Framework.Pipelines.Blocks.RegisteredPluginBlock>().After<RunningPluginsBlock>(); })
            );
            services.RegisterAllCommands(assembly);
        }
    }
}
