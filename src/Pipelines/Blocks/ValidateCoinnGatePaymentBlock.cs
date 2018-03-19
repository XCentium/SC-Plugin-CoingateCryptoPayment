using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Plugin.Payments;
using Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Pipelines.Arguments;
using Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Components;


namespace Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Pipelines.Blocks
{
    [PipelineDisplayName("Payments.block.validatefederatedpayment")]
    public class ValidateCoinnGatePaymentBlock : PipelineBlock<AddCoinGatePaymentArgument, Cart, CommercePipelineExecutionContext>
    {
        private readonly ValidatePartyCommand _validatePartyCommand;
        public ValidateCoinnGatePaymentBlock(ValidatePartyCommand validatePartyCommand)
         : base((string)null)
        {
            this._validatePartyCommand = validatePartyCommand;
        }

        public override async Task<Cart> Run(AddCoinGatePaymentArgument inputArgs , CommercePipelineExecutionContext context)
        {
            ValidateCoinnGatePaymentBlock federatedPaymentBlock = this;
            var arg = inputArgs.Cart;
            //Cart cart = inputArgs.Cart;
            arg.GetComponent<ContactComponent>().Email = inputArgs.Email;


            //Condition.Requires<AddCoinGatePaymentArgument>(inputArgs).IsNotNull<Cart>(string.Format("{0}: The cart can not be null", (federatedPaymentBlock.Name)));
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
                string defaultMessage = string.Format("Argument of type {0} was not found in context.", typeof(CartPaymentsArgument).Name);
                executionContext.Abort(await commerceContext.AddMessage(error, commerceTermKey, args, defaultMessage), (object)context);
                executionContext = (CommercePipelineExecutionContext)null;
                return (Cart)null;
            }
            IEnumerable<PaymentComponent> payments = argument.Payments;
            FederatedPaymentComponent payment = payments != null ? payments.OfType<FederatedPaymentComponent>().FirstOrDefault<FederatedPaymentComponent>() : (FederatedPaymentComponent)null;
            if (payment == null)
                return arg;
            bool isValid = true;
            if (string.IsNullOrEmpty(payment.PaymentMethodNonce))
            {
                string str = await context.CommerceContext.AddMessage(context.GetPolicy<KnownResultCodes>().Error, "InvalidOrMissingPropertyValue", new object[1]
                {
          (object) "PaymentMethodNonce"
                }, "Invalid or missing value for property 'PaymentMethodNonce'.");
                isValid = false;
            }
            Party billingParty = payment.BillingParty;
            if (billingParty != null)
            {
                FederatedPaymentComponent paymentComponent = payment;
                paymentComponent.BillingParty = await federatedPaymentBlock._validatePartyCommand.Process(context.CommerceContext, billingParty);
                paymentComponent = (FederatedPaymentComponent)null;
            }
            else
            {
                // ISSUE: explicit non-virtual call
                string str = await context.CommerceContext.AddMessage(context.GetPolicy<KnownResultCodes>().Error, "InvalidOrMissingPropertyValue", new object[1]
                {
                     (object) "payment.BillingParty"
                }, string.Format("{0}. Billing Party is not valid", (object)(federatedPaymentBlock.Name)));
                isValid = false;
            }
            if (!isValid)
            {
                executionContext = context;
                CommerceContext commerceContext = context.CommerceContext;
                string error = context.GetPolicy<KnownResultCodes>().Error;
                string commerceTermKey = "InvalidOrMissingPropertyValue";
                object[] args = new object[1]
                {
                     (object) "FederatedPayment"
                };
                
                string defaultMessage = string.Format("{0}. Federated Payment is not valid", (federatedPaymentBlock.Name));
                executionContext.Abort(await commerceContext.AddMessage(error, commerceTermKey, args, defaultMessage), (object)context);
                executionContext = (CommercePipelineExecutionContext)null;
            }


            arg.SetComponent(new CoingGatePaymentComponent() { CancelUrl = inputArgs.CancelUrl, SuccessUrl = inputArgs.SuccessUrl, CallbackUrl = inputArgs.CallBackUrl });
            
            return arg;
        }

    }
}
