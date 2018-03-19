# SC-Plugin-CoingateCryptoPayment
1. Add the plugin to the commerce solution, Add the plugin refrence to Sitecore.Commerce.Engine project and publish. This plugin refrences the plugin 'Plugin.Sample.Payments.Braintree' as well. So make sure it is also present in the solution before publishing.

2. Register the policy set for the environments where this pluing will be used {
        "$type": "Sitecore.Commerce.Core.PolicySetPolicy, Sitecore.Commerce.Core",
        "PolicySetId": "Entity-PolicySet-CoinGatePolicySet"
      },
3. Modify the file 'Plugin.CoinGate.PolicySet-1.0.0.json'. Add you coin gate environment details
4. Re-bootstrap
5. Once deployed, you will have the following apis available:
PUT {{ServiceHost}}/{{ShopsApi}}/AddCoinGatePayment() - Set the payment type to Crypto. Adds a component to Cart to save details returned from CoinGate
PUT {{ServiceHost}}/{{ShopsApi}}/PaymentCartBitcoin - Return the invoice url from CoinGate if available otherwise empty. This is where the user has to be redirected from the frontend to make payment. It the your responsibility to provide the success url and the add the logic to submit the cart when that success url is called.


Sample Body for AddCoinGatePayment 

{
    "cartId":"DefaultEntity-Customer-2d35101dd76342488be42e60b5e8309bStorefront",
    "payment": {
        "@odata.type": "Sitecore.Commerce.Plugin.Payments.FederatedPaymentComponent",
        "PaymentMethod": {"EntityTarget": "0CFFAB11-2674-4A18-AB04-228B1F8A1DEC", "Name": "Federated"},
        "PaymentMethodNonce": "212132",
        "Amount": { "Amount": 230.99 },
        "BillingParty": {"AddressName": "name", "FirstName": "firstname", "LastName":"lastname", "City": "city", "Address1": "123 st", "State": "ON", "Country": "CA", "ZipPostalCode":"123456"} 
    },
    "successUrl": "https://sxa.storefront.com/bitcoinsuccess",
    "cancelUrl" : "https://sxa.storefront.com/checkout/delivery",
    "callbackUrl" : "https://sxa.storefront.com/checkout/delivery",
    "email":"kautilya.prasad@gmail.com"
}


Sample Submit Order Code from Sitecore FE. This is the code that will be execute when the customer returns back from successful payment from CoinGate. i.e. Success URL
 
        
        public ActionResult SubmitBitcoinOrder()
        {
            SubmitVisitorOrderInputModel inputModel = new SubmitVisitorOrderInputModel() { ConfirmItemPath = "/sitecore/content/Sitecore/Storefront/Home/checkout/review" };
            BaseJsonResult result = new BaseJsonResult(this.StorefrontContext);
            this.ValidateModel(result);
            if (result.HasErrors)
                return this.Json((object)result);

            var submit = this.ReviewRepository.SubmitCurrentVisitorOrder(this.VisitorContext, inputModel);
            return Redirect(submit.NextPageLink);            
        }
