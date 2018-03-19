using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Core;

namespace Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Policies
{
    public class CoinGatePolicy : Policy
    {

        public CoinGatePolicy()
        {
            this.CoinGateUrl = string.Empty;
            this.AccessKey = string.Empty;
            this.SecretKey = string.Empty;
            this.AppId = string.Empty;
        }
        public string CoinGateUrl { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string AppId { get; set; }
    }
}
