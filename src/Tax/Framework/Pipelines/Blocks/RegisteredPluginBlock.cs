// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisteredPluginBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2018
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;

namespace Kjaneb.Commerce.Plugin.Tax.Framework.Pipelines.Blocks
{
    public class RegisteredPluginBlock : PipelineBlock<IEnumerable<RegisteredPluginModel>, IEnumerable<RegisteredPluginModel>, CommercePipelineExecutionContext>
    {
        public override Task<IEnumerable<RegisteredPluginModel>> Run(IEnumerable<RegisteredPluginModel> arg, CommercePipelineExecutionContext context)
        {
            if (arg == null)
            {
                return Task.FromResult((IEnumerable<RegisteredPluginModel>)null);
            }

            var plugins = arg.ToList();
            PluginHelper.RegisterPlugin(this, plugins);

            return Task.FromResult(plugins.AsEnumerable());
        }
    }
}
