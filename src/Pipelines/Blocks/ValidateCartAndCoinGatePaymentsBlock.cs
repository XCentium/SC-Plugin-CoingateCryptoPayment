using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Sitecore.Commerce.Plugin.Payments;
using Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Pipelines.Arguments;


namespace Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Pipelines.Blocks
{
    [PipelineDisplayName("CoinGateCryptoPayment.block.ValidateCartAndPayments")]
    public class ValidateCartAndCoinGatePaymentsBlock : PipelineBlock<AddCoinGatePaymentArgument, AddCoinGatePaymentArgument, CommercePipelineExecutionContext>
    {
        private readonly IGetCartPaymentMethodsPipeline _getMethodsPipeline;
        private readonly IGetPaymentMethodsPipeline _getAllMethodsPipeline;
        private readonly IGetCartPaymentOptionsPipeline _getOptionsPipeline;

        public ValidateCartAndCoinGatePaymentsBlock(IGetCartPaymentMethodsPipeline getCartPaymentMethodsPipeline, IGetCartPaymentOptionsPipeline getCartPaymentOptionsPipeline, IGetPaymentMethodsPipeline getPaymentMethodsPipeline)
            : base((string)null)
        {
            this._getMethodsPipeline = getCartPaymentMethodsPipeline;
            this._getOptionsPipeline = getCartPaymentOptionsPipeline;
            this._getAllMethodsPipeline = getPaymentMethodsPipeline;
        }


        public override async Task<AddCoinGatePaymentArgument> Run(AddCoinGatePaymentArgument inputArgs, CommercePipelineExecutionContext context)
        {
            var arg = new CartPaymentsArgument(inputArgs.Cart, inputArgs.Payments);
            ValidateCartAndCoinGatePaymentsBlock andPaymentsBlock = this;

            Condition.Requires<CartPaymentsArgument>(arg).IsNotNull<CartPaymentsArgument>(string.Format("{0}: The argument cannot be null.",andPaymentsBlock.Name));            
            Condition.Requires<Cart>(arg.Cart).IsNotNull<Cart>(string.Format("{0}: The cart cannot be null.", andPaymentsBlock.Name));            
            Condition.Requires<IEnumerable<PaymentComponent>>(arg.Payments).IsNotNull<IEnumerable<PaymentComponent>>(string.Format("{0}: The payments cannot be null.", andPaymentsBlock.Name));

            Cart cart = arg.Cart;
            CommercePipelineExecutionContext executionContext;
            if (!cart.Lines.Any<CartLineComponent>())
            {
                executionContext = context;
                CommerceContext commerceContext = context.CommerceContext;
                string error = context.CommerceContext.GetPolicy<KnownResultCodes>().Error;
                string commerceTermKey = "CartHasNoLines";
                object[] args = new object[1] { (object)cart.Id };
                string defaultMessage = string.Format("Cart '{0}' has no lines.", (object)cart.Id);
                executionContext.Abort(await commerceContext.AddMessage(error, commerceTermKey, args, defaultMessage), (object)context);
                executionContext = (CommercePipelineExecutionContext)null;
                return null;
            }
            List<PaymentOption> options = (await andPaymentsBlock._getOptionsPipeline.Run(new CartArgument(cart), context)).ToList<PaymentOption>();
            List<PaymentMethod> methods = (await andPaymentsBlock._getAllMethodsPipeline.Run(string.Empty, context)).ToList<PaymentMethod>();

            context.CommerceContext.AddUniqueObjectByType((object)arg);

            inputArgs.Cart = cart;

            return inputArgs;
        }

    }
}
