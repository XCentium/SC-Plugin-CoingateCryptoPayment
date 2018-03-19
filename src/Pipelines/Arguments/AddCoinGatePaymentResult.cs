using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Conditions;

namespace Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Pipelines.Arguments
{
    public class AddCoinGatePaymentResult
    {
        public string Id { get; set; }
        public string Currency { get; set; }
        public string Bitcoin_Uri { get; set; }
        public string Status { get; set; }
        public double Price { get; set; }
        public string Btc_Amount { get; set; }
        public string bitcoin_address { get; set; }
        public string Order_Id { get; set; }
        public string Payment_Url { get; set; }
        public string ErrorCode { get; set; }

    }
}
