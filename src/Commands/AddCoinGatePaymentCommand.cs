using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Pipelines;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Payments;
using Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Pipelines.Arguments;

namespace Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Commands
{
    public class AddCoinGatePaymentCommand : CommerceCommand
    {
        private readonly IAddCoinGatePaymentPipeline _addCoinGatePaymentPipeline;
        private readonly IFindEntityPipeline _getCartPipeline;

        public AddCoinGatePaymentCommand(IFindEntityPipeline getCartPipeline, IAddCoinGatePaymentPipeline addCoinGatePaymentPipeline, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _addCoinGatePaymentPipeline = addCoinGatePaymentPipeline;
            _getCartPipeline = getCartPipeline;
        }

        public virtual async Task<Cart> Process(CommerceContext commerceContext, string cartId, IEnumerable<PaymentComponent> payments, string successUrl, string cancelUrl, string callBackUrl, string email)
        {
            AddCoinGatePaymentCommand addPaymentsCommand = this;
            Cart result = (Cart)null;
            Cart cart1;

            using (CommandActivity.Start(commerceContext, (CommerceCommand)addPaymentsCommand))
            {
                Func<Task> func = await addPaymentsCommand.PerformTransaction(commerceContext, (Func<Task>)(async () =>
                {
                    CommercePipelineExecutionContextOptions pipelineContextOptions = commerceContext.GetPipelineContextOptions();

                    Cart cart = await this._getCartPipeline.Run(new FindEntityArgument(typeof(Cart), cartId, false), pipelineContextOptions) as Cart;

                    if (cart == null)
                    {
                        
                        string str = await commerceContext.AddMessage(commerceContext.GetPolicy<KnownResultCodes>().ValidationError, "EntityNotFound", new object[1]
                        {
                         (object) cartId
                        }, string.Format("Entity {0} was not found.", (object)cartId));
                    }
                    else
                    {
                        
                        Cart cart2 = await this._addCoinGatePaymentPipeline.Run(new AddCoinGatePaymentArgument(cart, payments, successUrl, cancelUrl, callBackUrl, email), commerceContext.GetPipelineContextOptions());                        
                        result = cart2;
                    }
                }));
                cart1 = result;
            }
            return cart1;
        }
    }
}
