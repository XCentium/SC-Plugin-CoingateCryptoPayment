using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Payments;

namespace Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Pipelines.Arguments
{
    public class AddCoinGatePaymentArgument : CartPaymentsArgument
    {
        public string CallBackUrl { get; set; }
        public string CancelUrl { get; set; }
        public string SuccessUrl { get; set; }
        public string Email { get; set; }

        public AddCoinGatePaymentArgument(Cart cart, IEnumerable<PaymentComponent> payments, string successUrl, string cancelUrl, string callBackUrl,string email) : base(cart, payments)
        {

            this.CallBackUrl = callBackUrl;
            this.SuccessUrl = successUrl;
            this.CancelUrl = cancelUrl;
            this.Email = email;
        }


    }
}
