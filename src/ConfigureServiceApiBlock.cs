// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureServiceApiBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.OData.Builder;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using Sitecore.Commerce.Plugin.Payments;
    using Sitecore.Commerce.Plugin.Carts;

    /// <summary>
    /// Defines a block which configures the OData model
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Microsoft.AspNetCore.OData.Builder.ODataConventionModelBuilder,
    ///         Microsoft.AspNetCore.OData.Builder.ODataConventionModelBuilder,
    ///         Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("SamplePluginConfigureServiceApiBlock")]
    public class ConfigureServiceApiBlock : PipelineBlock<ODataConventionModelBuilder, ODataConventionModelBuilder, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="modelBuilder">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="ODataConventionModelBuilder"/>.
        /// </returns>
        public override Task<ODataConventionModelBuilder> Run(ODataConventionModelBuilder modelBuilder, CommercePipelineExecutionContext context)
        {
            Condition.Requires(modelBuilder).IsNotNull($"{this.Name}: The argument cannot be null.");

            // Add the entities
            //modelBuilder.AddEntityType(typeof(SampleEntity));

            // Add the entity sets
            // modelBuilder.EntitySet<SampleEntity>("Sample");

            // Add complex types

            // Add unbound functions

            // Add unbound actions

            //modelBuilder.EntitySet<string>("PaymentCart");

            //Condition.Requires<ODataConventionModelBuilder>(modelBuilder).IsNotNull<ODataConventionModelBuilder>("The argument can not be null");
            //modelBuilder.AddEntityType(typeof(Cart));
            //modelBuilder.AddComplexType(typeof(Totals));
            //modelBuilder.AddComplexType(typeof(LineAdded));
            //modelBuilder.AddComplexType(typeof(LineUpdated));
            //modelBuilder.AddComplexType(typeof(LineQuantityPolicy));
            //modelBuilder.EntitySet<Cart>("BitcoinCarts");


            modelBuilder.Action("BitcoinCarts").Returns<string>();


            ActionConfiguration actionConfiguration2 = modelBuilder.Action("PaymentCartBitcoin");
            actionConfiguration2.Parameter<string>("cartId");
            string entitySetName4 = "Commands";
            actionConfiguration2.ReturnsFromEntitySet<CommerceCommand>(entitySetName4);



            ActionConfiguration actionConfiguration1 = modelBuilder.Action("AddCoinGatePayment");
            string name4 = "cartId";
            actionConfiguration1.Parameter<string>(name4);
            
            actionConfiguration1.Parameter<string>("successUrl");            
            actionConfiguration1.Parameter<string>("cancelUrl");            
            actionConfiguration1.Parameter<string>("callbackUrl");
            actionConfiguration1.Parameter<string>("email");
            string name5 = "payment";
            actionConfiguration1.Parameter<FederatedPaymentComponent>(name5);
            string entitySetName3 = "Commands";
            actionConfiguration1.ReturnsFromEntitySet<CommerceCommand>(entitySetName3);

            return Task.FromResult(modelBuilder);
        }
    }
}
