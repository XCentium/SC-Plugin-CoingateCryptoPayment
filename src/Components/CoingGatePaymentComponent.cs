using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;

namespace Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Components
{
    public class CoingGatePaymentComponent : Component
    {
        public string Identifier { get; set; }
        public string Currency { get; set; }
        public string BitcoinUri { get; set; }
        public string Status { get; set; }
        public double Price { get; set; }
        public string BtcAmount { get; set; }
        public string BitcoinAddress { get; set; }
        public string OrderId { get; set; }
        public string PaymentUrl { get; set; }
        public string ErrorCode { get; set; }
        public string SuccessUrl { get; set; }
        public string CancelUrl { get; set; }
        public string CallbackUrl { get; set; }
    }
}
