using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Logging;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Core.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Commands;

namespace Sitecore.Commerce.XC.Plugin.CoinGateCryptoPayment.Controllers
{
    [EnableQuery]
    [Route("api/BitcoinCarts")]
    public class CoinGateController : CommerceController
    {
        public CoinGateController(IServiceProvider serviceProvider, CommerceEnvironment globalEnvironment)
            : base(serviceProvider, globalEnvironment)
        {
        }

        [EnableQuery]
        [Route("BitcoinCarts")]
        public async Task<string> Get()
        {
            CoinGateController cartsController = this;
            cartsController.Logger.LogDebug(string.Format("CartsController_Get: {0}", (object)CommerceEntity.ListName<Cart>()), Array.Empty<object>());
            FindEntitiesInListCommand entitiesInListCommand = cartsController.Command<FindEntitiesInListCommand>();
            Task<CommerceList<Cart>> task;
            if (entitiesInListCommand == null)
            {
                task = (Task<CommerceList<Cart>>)null;
            }
            else
            {
                CommerceContext currentContext = cartsController.CurrentContext;
                string listName = CommerceEntity.ListName<Cart>();
                int skip = 0;
                int maxValue = int.MaxValue;
                task = entitiesInListCommand.Process<Cart>(currentContext, listName, skip, maxValue);
            }
            CommerceList<Cart> commerceList = await task;
            return "";//(IEnumerable<Cart>)((commerceList != null ? commerceList.Items.ToList<Cart>() : (List<Cart>)null) ?? new List<Cart>());
        }

        [HttpGet]
        [Route("Id={id}")]
        [EnableQuery]
        public async Task<string> Get(string id)
        {
            CoinGateController cartsController = this;
            if (!cartsController.ModelState.IsValid || string.IsNullOrEmpty(id))
                return "";
            string result = await cartsController.Command<GetBitcoinPaymentRedirectUrlCommand>().Process(cartsController.CurrentContext, id).ConfigureAwait(false);
            return result != null ? result : "";
        }
    }
}
