using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Kjaneb.Commerce.Plugin.Tax.TaxJar.Pipelines.Blocks
{
    public class RegisteredPluginBlock : SyncPipelineBlock<IEnumerable<RegisteredPluginModel>, IEnumerable<RegisteredPluginModel>, CommercePipelineExecutionContext>
    {
        public override IEnumerable<RegisteredPluginModel> Run(IEnumerable<RegisteredPluginModel> arg, CommercePipelineExecutionContext context)
        {
            if (arg == null)
            {
                return null;
            }

            var plugins = arg.ToList();
            PluginHelper.RegisterPlugin(this, plugins);

            return plugins.AsEnumerable();
        }
    }
}
