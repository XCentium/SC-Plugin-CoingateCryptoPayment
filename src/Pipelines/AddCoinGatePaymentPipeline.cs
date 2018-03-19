using Sitecore.Framework.Pipelines;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Payments;
using Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Pipelines.Arguments;

namespace Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Pipelines
{

    public class AddCoinGatePaymentPipeline : CommercePipeline<AddCoinGatePaymentArgument, Cart>, IAddCoinGatePaymentPipeline, IPipeline<AddCoinGatePaymentArgument, Cart, CommercePipelineExecutionContext>, IPipelineBlock<AddCoinGatePaymentArgument, Cart, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
        public AddCoinGatePaymentPipeline(IPipelineConfiguration<IAddCoinGatePaymentPipeline> configuration, ILoggerFactory loggerFactory)
          : base((IPipelineConfiguration)configuration, loggerFactory)
        {
        }
    }
}

