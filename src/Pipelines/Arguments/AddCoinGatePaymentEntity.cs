// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SampleArgument.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Conditions;
    


    /// <inheritdoc />
    /// <summary>
    /// Defines an argument
    /// </summary>
    /// <seealso cref="T:Sitecore.Commerce.Core.PipelineArgument" />    
    public class AddCoinGatePaymentEntity 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddCoinGatePaymentEntity"/> class.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public AddCoinGatePaymentEntity()
        {
           
        }

        /// <summary>
        /// Gets or sets the parameter.
        /// </summary>
        /// <value>
        /// The parameter.
        /// </value>
        public string  OrderId { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public string ReceiveCurrency { get; set; }
        public string Description { get; set; }
        public string CallBackUrl { get; set; }
        public string CancelUrl { get; set; }
        public string SuccessUrl { get; set; }
    }
}
