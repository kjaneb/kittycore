using System.Reflection;
using Kjaneb.Commerce.Plugin.Tax.Framework.Pipelines;
using Kjaneb.Commerce.Plugin.Tax.TaxJar.Pipelines.Blocks;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines.Definitions.Extensions;

namespace Kjaneb.Commerce.Plugin.Tax.TaxJar
{
    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config
                .ConfigurePipeline<ICallExternalServiceToCalculateTaxPipeline>(builder => builder
                        .Add<CalculateTaxWithTaxJarBlock>()
                , "main", 100)
            );
            services.RegisterAllCommands(assembly);
        }
    }
}