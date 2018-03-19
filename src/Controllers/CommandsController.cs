// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandsController.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Http.OData;

    using Microsoft.AspNetCore.Mvc;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Commands;
    using Sitecore.Commerce.Plugin.Payments;
    using Sitecore.Commerce.Plugin.Carts;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;

    /// <inheritdoc />
    /// <summary>
    /// Defines a controller
    /// </summary>
    /// <seealso cref="T:Sitecore.Commerce.Core.CommerceController" />
    public class CommandsController : CommerceController
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Commerce.Plugin.Sample.CommandsController" /> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="globalEnvironment">The global environment.</param>
        public CommandsController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment)
            : base(serviceProvider, globalEnvironment)
        {
        }

        /// <summary>
        /// Samples the command.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="IActionResult"/></returns>
        [HttpPut]
        [Route("AddCoinGatePayment()")]
        public async Task<IActionResult> AddCoinGatePayment([FromBody] ODataActionParameters value)
        {

            CommandsController commandsController = this;
            if (!commandsController.ModelState.IsValid || value == null)
                return (IActionResult)new BadRequestObjectResult(commandsController.ModelState);
            if (value.ContainsKey("cartId"))
            {
                object obj1 = value["cartId"];

                var successUrl = value["successUrl"].ToString();
                var cancelUrl = value["cancelUrl"].ToString();
                var callbackUrl = value["callbackUrl"].ToString();
                var email = value["email"].ToString();

                if (!string.IsNullOrEmpty(obj1 != null ? obj1.ToString() : (string)null) && value.ContainsKey("payment"))
                {
                    object obj2 = value["payment"];
                    if (!string.IsNullOrEmpty(obj2 != null ? obj2.ToString() : (string)null))
                    {
                        string cartId = value["cartId"].ToString();
                        FederatedPaymentComponent paymentComponent = JsonConvert.DeserializeObject<FederatedPaymentComponent>(value["payment"].ToString());
                        AddCoinGatePaymentCommand command = commandsController.Command<AddCoinGatePaymentCommand>();
                        Cart cart = await command.Process(commandsController.CurrentContext, cartId, (IEnumerable<PaymentComponent>)new List<PaymentComponent>()
                            {
                              (PaymentComponent) paymentComponent
                            }, successUrl, cancelUrl, callbackUrl, email);
                        return (IActionResult)new ObjectResult((object)command);
                    }
                }
            }
            return (IActionResult)new BadRequestObjectResult((object)value);
        }

        [HttpPut]
        [Route("PaymentCartBitcoin()")]
        public async Task<string> PaymentCartBitcoin([FromBody] ODataActionParameters value)
        {

            CommandsController commandsController = this;
            if (!commandsController.ModelState.IsValid || value == null)
                return "";
            if (value.ContainsKey("cartId"))
            {
                object obj1 = value["cartId"];

                if (!string.IsNullOrEmpty(obj1 != null ? obj1.ToString() : ""))
                {

                    string cartId = value["cartId"].ToString();
                    GetBitcoinPaymentRedirectUrlCommand command = commandsController.Command<GetBitcoinPaymentRedirectUrlCommand>();
                    string url = await command.Process(commandsController.CurrentContext, cartId);
                    return url;

                }
            }
            return "";
        }
    }
}

