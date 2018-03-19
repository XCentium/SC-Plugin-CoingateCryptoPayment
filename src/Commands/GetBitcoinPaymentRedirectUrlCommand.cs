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
using Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Components;

namespace Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Commands
{
    public class GetBitcoinPaymentRedirectUrlCommand : CommerceCommand
    {
        private readonly IAddCoinGatePaymentPipeline _addCoinGatePaymentPipeline;
        private readonly IFindEntityPipeline _getCartPipeline;

        public GetBitcoinPaymentRedirectUrlCommand(IFindEntityPipeline getCartPipeline, IAddCoinGatePaymentPipeline addCoinGatePaymentPipeline, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _addCoinGatePaymentPipeline = addCoinGatePaymentPipeline;
            _getCartPipeline = getCartPipeline;
        }

        public virtual async Task<string> Process(CommerceContext commerceContext, string cartId)
        {
            GetBitcoinPaymentRedirectUrlCommand getBitcoinPaymentRedirectUrlCommand = this;
            Cart result = (Cart)null;
            Cart cart1;
            string returnUrl = "";

            using (CommandActivity.Start(commerceContext, (CommerceCommand)getBitcoinPaymentRedirectUrlCommand))
            {
                Func<Task> func = await getBitcoinPaymentRedirectUrlCommand.PerformTransaction(commerceContext, (Func<Task>)(async () =>
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
                        if(cart.HasComponent<CoingGatePaymentComponent>())
                        {
                            var coinGateComponent = cart.GetComponent<CoingGatePaymentComponent>();
                            returnUrl = coinGateComponent.PaymentUrl;
                        }                        
                    }
                }));
                cart1 = result;
            }
            return returnUrl;
        }
    }
}
