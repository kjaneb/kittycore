using System;
using System.Linq;
using System.Threading.Tasks;
using Kjaneb.Commerce.Plugin.Tax.Framework.Models;
using Kjaneb.Commerce.Plugin.Tax.TaxJar.Policies;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using Taxjar;

namespace Kjaneb.Commerce.Plugin.Tax.TaxJar.Pipelines.Blocks
{
    public class CalculateTaxWithTaxJarBlock : PipelineBlock<TaxableCartModel, TaxBreakdownModel, CommercePipelineExecutionContext>
    {
        private CommerceCommander _commerceCommander;

        public CalculateTaxWithTaxJarBlock(CommerceCommander commerceCommander)
        {
            _commerceCommander = commerceCommander;
        }

        public override Task<TaxBreakdownModel> Run(TaxableCartModel arg, CommercePipelineExecutionContext context)
        {
            var taxJarPolicy = context.GetPolicy<TaxJarPolicy>();
            var client = new TaxjarApi(taxJarPolicy.ApiKey);

            var nexus = arg.NexusAddresses.Select(x => new
            {
                id = Guid.NewGuid(),
                country = x.Country,
                zip = x.Zip,
                state = x.State,
                city = x.City,
                street = x.Address
            }).ToArray();

            var lineItems = arg.LineItems.Select(x => new
            {
                    id = x.Id,
                    quantity = x.Quantity,
                    unit_price = x.Price,
                    discount = x.Discount
            }).ToArray();

            var response = client.TaxForOrder(new
            {
                to_country = arg.ToCountry,
                to_zip = arg.ToZip,
                to_state = arg.ToState,
                to_city = arg.ToCity,
                to_street = arg.ToStreet,
                amount = arg.Amount,
                shipping = arg.Shipping,
                line_items = lineItems.ToArray(),
                nexus_addresses = nexus
            });

            context.Logger.Log(LogLevel.Information, "Total: " + response.TaxableAmount + " shipping: " + response.Shipping + " Tax Payable: " + response.AmountToCollect + " Has Nexus: " + response.HasNexus);

            var result = new TaxBreakdownModel();

            if (response != null)
            {
                result.HasNexus = response.HasNexus;
                result.IsShippingTaxable = response.FreightTaxable;
                result.AmountToCollect = response.AmountToCollect;
            }

            return Task.FromResult(result);
        }
    }
}
