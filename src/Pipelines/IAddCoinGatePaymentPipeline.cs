using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Pipelines;
using Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment;
using Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Pipelines.Arguments;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Payments;

namespace Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Pipelines
{
    public interface IAddCoinGatePaymentPipeline : IPipeline<AddCoinGatePaymentArgument, Cart, CommercePipelineExecutionContext>, IPipelineBlock<AddCoinGatePaymentArgument, Cart, CommercePipelineExecutionContext>, IPipelineBlock, IPipeline
    {
    }
}
