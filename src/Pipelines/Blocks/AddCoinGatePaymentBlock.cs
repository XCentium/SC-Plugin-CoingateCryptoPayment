// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Pipelines.Blocks
{
    using System;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment;
    using Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Pipelines.Arguments;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Security.Cryptography;
    using Newtonsoft.Json;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.Payments;
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Plugin.Pricing;
    using System.Linq;
    using Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Components;



    /// <summary>
    /// Defines a block
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Plugin.Sample.SampleArgument,
    ///         Sitecore.Commerce.Plugin.Sample.SampleEntity, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("CoinGateCryptoPayment.Pipelines.Blocks.AddCoinGatePaymentBlock")]
    public class AddCoinGatePaymentBlock : PipelineBlock<Cart, Cart, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="arg">
        /// The SampleArgument argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="SampleEntity"/>.
        /// </returns>
        /// 
        public AddCoinGatePaymentBlock()
        : base((string)null)
        {
        }


        public override async Task<Cart> Run(Cart arg, CommercePipelineExecutionContext context)
        {

            AddCoinGatePaymentBlock addPaymentsBlock = this;
            // ISSUE: explicit non-virtual call
            Condition.Requires<Cart>(arg).IsNotNull<Cart>(string.Format("{0}: The argument cannot be null.", addPaymentsBlock.Name));
            CartPaymentsArgument argument = context.CommerceContext.GetObject<CartPaymentsArgument>();
            CommercePipelineExecutionContext executionContext;
            if (argument == null)
            {
                executionContext = context;
                CommerceContext commerceContext = context.CommerceContext;
                string error = context.GetPolicy<KnownResultCodes>().Error;
                string commerceTermKey = "ArgumentNotFound";
                object[] args = new object[1]
                {
                    (object) typeof (CartPaymentsArgument).Name
                };
                string defaultMessage = string.Format("Argument of type {0} was not found in context.", (object)typeof(CartPaymentsArgument).Name);
                executionContext.Abort(await commerceContext.AddMessage(error, commerceTermKey, args, defaultMessage), (object)context);
                executionContext = (CommercePipelineExecutionContext)null;
                return (Cart)null;
            }
            Cart cart = argument.Cart;
            foreach (PaymentComponent payment in argument.Payments)
            {
                PaymentComponent p = payment;
                if (p != null)
                {
                    if (string.IsNullOrEmpty(p.Id))
                        p.Id = Guid.NewGuid().ToString("N");
                    // ISSUE: explicit non-virtual call
                    context.Logger.LogInformation(string.Format("{0} - Adding Payment {1} Amount:{2}", (addPaymentsBlock.Name), (object)p.Id, (object)p.Amount.Amount), Array.Empty<object>());
                    if (string.IsNullOrEmpty(p.Amount.CurrencyCode))
                        p.Amount.CurrencyCode = context.CommerceContext.CurrentCurrency();
                    else if (!p.Amount.CurrencyCode.Equals(context.CommerceContext.CurrentCurrency(), StringComparison.OrdinalIgnoreCase))
                    {
                        executionContext = context;
                        CommerceContext commerceContext = context.CommerceContext;
                        string error = context.GetPolicy<KnownResultCodes>().Error;
                        string commerceTermKey = "InvalidCurrency";
                        object[] args = new object[2]
                        {
                              (object) p.Amount.CurrencyCode,
                              (object) context.CommerceContext.CurrentCurrency()
                        };
                        string defaultMessage = string.Format("Invalid currency '{0}'. Valid currency is '{1}'.", (object)p.Amount.CurrencyCode, (object)context.CommerceContext.CurrentCurrency());
                        executionContext.Abort(await commerceContext.AddMessage(error, commerceTermKey, args, defaultMessage), (object)context);
                        executionContext = (CommercePipelineExecutionContext)null;
                        return (Cart)null;
                    }
                    if (context.GetPolicy<GlobalPricingPolicy>().ShouldRoundPriceCalc)
                    {
                        p.Amount.Amount = Decimal.Round(p.Amount.Amount, context.GetPolicy<GlobalPricingPolicy>().RoundDigits, context.GetPolicy<GlobalPricingPolicy>().MidPointRoundUp ? MidpointRounding.AwayFromZero : MidpointRounding.ToEven);                        
                        context.Logger.LogDebug(string.Format("{0} - After Rounding: {1}",(addPaymentsBlock.Name), (object)p.Amount.Amount), Array.Empty<object>());
                    }

                    p.Comments = "Bitcoin";     
                    
                    cart.SetComponent((Component)p);
                    p = (PaymentComponent)null;
                }
            }
            
            
            return cart;
        }
        
    }
}
