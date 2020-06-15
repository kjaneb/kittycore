using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;

namespace Kjaneb.Commerce.Plugin.Tax.Framework.Models
{
    public class TaxBreakdownModel
    {
        public decimal AmountToCollect { get; set; }
        public decimal TaxableAmount { get; set; }

        public decimal CombinedTaxRate { get; set; }
        public bool IsShippingTaxable { get; set; }
        public bool HasNexus { get; set; }
    }
}
