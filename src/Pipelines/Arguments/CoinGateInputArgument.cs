using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Pipelines.Arguments
{
    public class CoinGateInputArgument
    {
        public string order_id { get; set; }
        public decimal price { get; set; }
        public string currency { get; set; }
        public string receive_currency { get; set; }
        public string description { get; set; }
        public string callback_url { get; set; }
        public string cancel_url { get; set; }
        public string success_url { get; set; }
    }
}
