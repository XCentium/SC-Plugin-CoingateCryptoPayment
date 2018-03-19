// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Pipelines.Blocks
{
    using System;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;
    using Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment;
    using Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Pipelines.Arguments;
    using System.Net.Http;
    using System.Text;
    using System.Security.Cryptography;
    using Newtonsoft.Json;
    using Sitecore.Commerce.Plugin.Carts;
    using System.Linq;
    using Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Components;
    using System.Collections;
    using System.Collections.Generic;
    using Newtonsoft.Json.Linq;
    using Sitecore.Commerce.Plugin.Payments;
    using Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Policies;




    /// <summary>
    /// Defines a block
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Plugin.Sample.SampleArgument,
    ///         Sitecore.Commerce.Plugin.Sample.SampleEntity, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("CoinGateCryptoPayment.Pipelines.Blocks.CallCoinGateServiceBlock")]
    public class CallCoinGateServiceBlock : PipelineBlock<Cart, Cart, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="arg">
        /// The SampleArgument argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="SampleEntity"/>.
        /// </returns>
        /// 
        public CallCoinGateServiceBlock()
        : base((string)null)
        {
        }


        public override async Task<Cart> Run(Cart arg, CommercePipelineExecutionContext context)
        {

            CallCoinGateServiceBlock addPaymentsBlock = this;
            Cart cart = arg;

            // Coin Gate
            CoinGateInputArgument coinGateArgument = new CoinGateInputArgument();
            coinGateArgument.description = string.Join(",", arg.Lines.Select(x => x.GetComponent<CartProductComponent>().ProductName + " - " + x.Quantity).ToList());
            coinGateArgument.order_id = arg.Id;
            coinGateArgument.receive_currency = "BTC";
            coinGateArgument.currency = context.CommerceContext.CurrentCurrency();
            coinGateArgument.price = arg.Totals.PaymentsTotal.Amount;

            CommercePipelineExecutionContext executionContext = context;
            if (!arg.HasComponent<CoingGatePaymentComponent>())
            {
                executionContext = context;
                CommerceContext commerceContext = context.CommerceContext;
                string error = context.GetPolicy<KnownResultCodes>().Error;
                string commerceTermKey = "InvalidOrMissingPropertyValue";
                object[] args = new object[1]
                {
                     (object) "CoingGatePaymentComponent"
                };

                string defaultMessage = string.Format("{0}. CoingGatePaymentComponent is not valid", (addPaymentsBlock.Name));
                executionContext.Abort(await commerceContext.AddMessage(error, commerceTermKey, args, defaultMessage), (object)context);
                executionContext = (CommercePipelineExecutionContext)null;
            }

            var coinGateComponentSaved = arg.GetComponent<CoingGatePaymentComponent>();

            
            coinGateArgument.success_url = coinGateComponentSaved.SuccessUrl;
            coinGateArgument.cancel_url = coinGateComponentSaved.CancelUrl;
            coinGateArgument.callback_url = coinGateComponentSaved.CallbackUrl;            

            var input = coinGateArgument.ToKeyValue();
                       
            var result = Task.FromResult(InvokeHttpClientPut(input, context)).Result;

            if (string.IsNullOrEmpty(result.Result.ErrorCode))
            {
                var data = result.Result;
                CoingGatePaymentComponent coinGateComponent = new CoingGatePaymentComponent()
                {
                    BtcAmount = data.Btc_Amount,
                    BitcoinAddress = data.bitcoin_address,
                    BitcoinUri = data.Bitcoin_Uri,
                    Identifier = data.Id,
                    OrderId = data.Order_Id,
                    PaymentUrl = data.Payment_Url,
                    Status = data.Status,
                    Price = data.Price
                };
                cart.SetComponent(coinGateComponent);                
            }           

            return cart;
        }


        public async Task<AddCoinGatePaymentResult> InvokeHttpClientPut(IDictionary<string, string> input, CommercePipelineExecutionContext context)
        {
            try
            {
                using (HttpClient client = this.GetClient(context))
                {

                    //var request = new HttpRequestMessage(HttpMethod.Post, "/path/to/post/to");
                    var content = new FormUrlEncodedContent(input);
                    content.Headers.Remove("Content-Type");
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");


                    HttpResponseMessage result = client.PostAsync("/v1/orders", content).Result;
                    if (result.IsSuccessStatusCode)
                        return JsonConvert.DeserializeObject<AddCoinGatePaymentResult>(result.Content.ReadAsStringAsync().Result); //result.Content.ReadAsStringAsync().Result;     
                    else
                    {
                        return new AddCoinGatePaymentResult() { ErrorCode = result.ReasonPhrase + " - " + result.Content.ReadAsStringAsync().Result };
                    }
                }
            }
            catch (Exception ex)
            {
                // tODO - log error    
                context.Abort(
                   await context.CommerceContext.AddMessage(
                       context.GetPolicy<KnownResultCodes>().Error,
                       "CoinGateParamsInvalid",
                       new object[] { ex.Message },
                       ex.StackTrace),
                   context);
                //return arg;

                return new AddCoinGatePaymentResult() { ErrorCode = ex.Message + " - " + ex.StackTrace };

            }
        }

        private HttpClient GetClient(CommercePipelineExecutionContext context)
        {

            // Create Coing Gate Policy to store these data

            var policy = context.GetPolicy<CoinGatePolicy>();

            string coinGateUrl = policy.CoinGateUrl; //"https://api-sandbox.coingate.com/";
            var accessKey = policy.AccessKey; //"3pEMPgvsNCjeKDnqHWGit2";
            var accessNonce =  new NonceGen().GenerateNonce();
            var secretKey = policy.SecretKey;
            var appId = policy.AppId;


            var accessSignature = GetAccessSignature(accessNonce, appId, accessKey, secretKey);

            HttpClient httpClient;
            httpClient = new HttpClient()
            {
                BaseAddress = new Uri(coinGateUrl)
            };

            //httpClient.DefaultRequestHeaders.Accept.Add("Content-Type", "application/x-www-form-urlencoded");
            httpClient.DefaultRequestHeaders.Add("Access-Key", accessKey);
            httpClient.DefaultRequestHeaders.Add("Access-Nonce", accessNonce);
            httpClient.DefaultRequestHeaders.Add("Access-Signature", accessSignature);
            httpClient.Timeout = new TimeSpan(0, 0, 60);

            return httpClient;
        }

        private string GetAccessSignature(string nonce, string appId, string apiKey, string secret)
        {
            String newNessage = nonce + appId + apiKey;
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

            var keyByte = encoding.GetBytes(secret);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                hmacsha256.ComputeHash(encoding.GetBytes(newNessage));
                return ByteToString(hmacsha256.Hash);
            }
        }

        static string ByteToString(byte[] buff)
        {
            string sbinary = "";
            for (int i = 0; i < buff.Length; i++)
                sbinary += buff[i].ToString("X2"); /* hex format */
            return sbinary.ToLower();
        }
    }


    public class NonceGen
    {
        private static readonly Random random = new Random();
        private static readonly object randLock = new object();

        public virtual string GenerateNonce()
        {
            lock (randLock)
            {
                // Just a simple implementation of a random number between 123400 and 9999999
                return random.Next(1, 66200).ToString();
            }
        }
    }

    public static class KP
    {
        public static IDictionary<string, string> ToKeyValue(this object metaToken)
        {
            if (metaToken == null)
            {
                return null;
            }

            JToken token = metaToken as JToken;
            if (token == null)
            {
                return ToKeyValue(JObject.FromObject(metaToken));
            }

            if (token.HasValues)
            {
                var contentData = new Dictionary<string, string>();
                foreach (JToken child in token.Children().ToList())
                {
                    var childContent = child.ToKeyValue();
                    if (childContent != null)
                    {
                        contentData = contentData.Concat(childContent)
                                                 .ToDictionary(k => k.Key, v => v.Value);
                    }
                }

                return contentData;
            }

            var jValue = token as JValue;
            if (jValue?.Value == null)
            {
                return null;
            }

            var value = jValue?.Type == JTokenType.Date ?
                            jValue?.ToString("o", System.Globalization.CultureInfo.InvariantCulture) :
                            jValue?.ToString(System.Globalization.CultureInfo.InvariantCulture);

            return new Dictionary<string, string> { { token.Path, value } };
        }
    }
}
