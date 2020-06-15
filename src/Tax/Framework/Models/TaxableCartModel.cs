using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;

namespace Kjaneb.Commerce.Plugin.Tax.Framework.Models
{
    public class TaxableCartModel
    {
        public string ToCountry { get; set; }
        public string ToZip { get; set; }
        public string ToState { get; set; }
        public string ToCity { get; set; }
        public string ToStreet { get; set; }
        public decimal Amount { get; set; }
        public decimal Shipping { get; set; }

        public List<TaxableLineItem> LineItems { get; set; }

        public List<NexusAddress> NexusAddresses { get; set; }
    }

    public class TaxableLineItem
    {
        public string Id { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
    }
}
