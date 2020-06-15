using Kjaneb.Commerce.Plugin.Tax.Framework.Components;
using Kjaneb.Commerce.Plugin.Tax.Framework.Models;
using Kjaneb.Commerce.Plugin.Tax.Framework.Policies;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Fulfillment;
using Sitecore.Commerce.Plugin.Pricing;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kjaneb.Commerce.Plugin.Tax.Framework.Pipelines.Blocks
{
    public class CalculateCartTaxBlock : PipelineBlock<Cart, Cart, CommercePipelineExecutionContext>
    {
        private CommerceCommander _commerceCommander;

        public CalculateCartTaxBlock(CommerceCommander commerceCommander)
        {
            _commerceCommander = commerceCommander;
        }

        public override async Task<Cart> Run(Cart arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull(Name + ": The cart can not be null");
            //Only calculate tax if there is a shipping address provided
            if (!arg.HasComponent<FulfillmentComponent>())
                return arg;
            //Don't calculate if there is a split shipment
            if (arg.GetComponent<FulfillmentComponent>() is SplitFulfillmentComponent)
                return arg;

            if (!arg.Lines.Any())
            {
                //if there are no cart lines, remove the tax calculation from the cart
                arg.Adjustments.Where(a =>
                {
                    if (!string.IsNullOrEmpty(a.Name) && a.Name.Equals("TaxFee", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(a.AdjustmentType))
                        return a.AdjustmentType.Equals(context.GetPolicy<KnownCartAdjustmentTypesPolicy>().Tax, StringComparison.OrdinalIgnoreCase);
                    return false;
                }).ToList().ForEach(a => arg.Adjustments.Remove(a));
                return arg;
            }

            var taxJarPolicy = context.GetPolicy<TaxFrameworkPolicy>();
            var fulfillment = arg.GetComponent<PhysicalFulfillmentComponent>();

            if (fulfillment != null && fulfillment.ShippingParty != null)
            {
                var address = fulfillment.ShippingParty;
                var lines = arg.Lines;

                try
                {
                    var model = new TaxableCartModel()
                    {
                        LineItems = new List<TaxableLineItem>(), 
                        NexusAddresses = new List<NexusAddress>(),
                        ToStreet = address.Address1,
                        ToCity = address.City,
                        ToState = address.StateCode,
                        ToCountry = address.CountryCode,
                        ToZip = address.ZipPostalCode
                    };

                    //Cart level discounts are marked as non-taxable, for more accurate tax calculations we should total cart discounts (excluding discounts to shipping) to split amoungst cart lines
                    var adjustments = arg.Adjustments
                        .Where(p => p.IsTaxable || p.AdjustmentType ==
                                    context.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount && !taxJarPolicy.FulfillmentDiscountActions.Contains(p.AwardingBlock))
                        .Aggregate(decimal.Zero, (current, adjustment) => current + adjustment.Adjustment.Amount);

                    //Spread cart level discounts across each item
                    var cartLineAdjustment = adjustments / lines.Count;
                    var subTotal = 0M;
                    foreach (var line in lines)
                    {
                        var lineTotal = line.Totals.SubTotal.Amount;
                        var lineAdjustments = line.Adjustments.Where(p => p.IsTaxable).Aggregate(Decimal.Zero,
                            (current, adjustment) => current + adjustment.Adjustment.Amount);

                        subTotal += (lineTotal + lineAdjustments + cartLineAdjustment);

                        var lineItem = new TaxableLineItem()
                        {
                            Id = line.Id,
                            Quantity = line.Quantity,
                            Price = line.Totals.SubTotal.Amount,
                            Discount = (lineAdjustments + cartLineAdjustment) * -1,
                        };

                        model.LineItems.Add(lineItem);
                    }

                    //Shipping is considered taxable in some states. Let the tax provider figure out what should be considered taxable
                    var shipping = arg.Adjustments
                        .Where(p => p.AdjustmentType == context.GetPolicy<KnownCartAdjustmentTypesPolicy>().Fulfillment)
                        .Aggregate(Decimal.Zero, (current, adjustment) => current + adjustment.Adjustment.Amount);

                    //Subtract discounts that alter shipping
                    var shippingDiscounts = arg.Adjustments.Where(x => x.AdjustmentType == context.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount && taxJarPolicy.FulfillmentDiscountActions.Contains(x.AwardingBlock))
                        .Aggregate(Decimal.Zero, (current, adjustment) => current + adjustment.Adjustment.Amount);

                    model.Shipping = shipping + shippingDiscounts;

                    var nexusAddresses = await _commerceCommander.Pipeline<IPopulateNexusAddressPipeline>().Run(arg, context);
                    model.NexusAddresses = nexusAddresses.ToList();

                    model.Amount = arg.Totals.GrandTotal.Amount - shipping;
                    model.Shipping = shipping;

                    if (model.Amount > 0)
                    {
                        context.Logger.Log(LogLevel.Information,
                            "Total: " + model.Amount + " shipping: " + model.Shipping +
                            " Line item total: " + subTotal);

                        var result = await _commerceCommander.Pipeline<ICallExternalServiceToCalculateTaxPipeline>()
                            .Run(model, context);

                        if (result != null)
                        {

                            context.Logger.Log(LogLevel.Information,
                                "Total: " + result.TaxableAmount + " shipping: " + result.IsShippingTaxable +
                                " Tax Payable: " + result.AmountToCollect + " Has Nexus: " + result.HasNexus);

                            var awardedAdjustment = new CartLevelAwardedAdjustment();
                            awardedAdjustment.Name = "TaxFee";
                            awardedAdjustment.DisplayName = "TaxFee";
                            awardedAdjustment.Adjustment = new Money(result.AmountToCollect);
                            awardedAdjustment.AdjustmentType = context.GetPolicy<KnownCartAdjustmentTypesPolicy>().Tax;
                            awardedAdjustment.AwardingBlock = (Name);
                            awardedAdjustment.IsTaxable = (false);
                            arg.Adjustments.Add(awardedAdjustment);

                            arg.Adjustments
                                .Where(p => p.AdjustmentType ==
                                            context.GetPolicy<KnownCartAdjustmentTypesPolicy>().Fulfillment)
                                .ForEach(x => x.IsTaxable = result.IsShippingTaxable);
                        }
                    }
                }
                catch (Exception ex)
                {
                    context.Logger.LogCritical(ex, "Error calculating tax with Framework provider");
                }
            }

            return arg;
        }
    }
}
