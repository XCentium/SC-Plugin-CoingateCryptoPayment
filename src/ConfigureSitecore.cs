// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSitecore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
using Plugin.Sample.Payments.Braintree;
namespace Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;
    using Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Pipelines;
    using Sitecore.Commerce.Plugin.Payments;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Pipelines.Blocks;
    using Sitecore.Commerce.Plugin.Orders;
    


    /// <summary>
    /// The configure sitecore class.
    /// </summary>
    public class ConfigureSitecore : IConfigureSitecore
    {
        /// <summary>
        /// The configure services.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config

             .AddPipeline<IAddCoinGatePaymentPipeline, AddCoinGatePaymentPipeline>(
                    configure =>
                        {
                            configure.Add<ValidateCartAndCoinGatePaymentsBlock>()
                            .Add<ValidateCoinnGatePaymentBlock>()
                            .Add<AddCoinGatePaymentBlock>()
                            .Add<ICalculateCartLinesPipeline>()
                            .Add<ICalculateCartPipeline>()
                            .Add<CallCoinGateServiceBlock>()
                            .Add<PersistCartBlock>()
                            .Add<WriteCartTotalsToContextBlock>();
                        })

                .ConfigurePipeline<ICreateOrderPipeline>(builder => builder.Replace<CreateFederatedPaymentBlock, CreateFederatedPaymentBlockCoinGate>())

               .ConfigurePipeline<IConfigureServiceApiPipeline>(configure => configure.Add<ConfigureServiceApiBlock>()));

            services.RegisterAllCommands(assembly);
        }
    }
}