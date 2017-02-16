using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Core.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Core.Controllers
{
    public class BasketController
    {
        private IStockManager _stockManager;
        private IBasketManager _basketManager;

        public BasketController(
            IStockManager stockManager,
            IBasketManager basketManager)
        {
            _stockManager = stockManager;
            _basketManager = basketManager;
        }

        [HttpGet("api/{userId}/basket")]
        public IActionResult GetBasket(
            [FromRoute]int userId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("api/{userId}/basket/add")]
        public IActionResult AddToBasket(
            [FromRoute]string userId,
            [FromQuery]int? productId = null,
            [FromQuery]string productName = null)
        {
            throw new NotImplementedException();
        }

        [HttpGet("api/{userId}/basket/remove")]
        public IActionResult RemoveFromBasket(
            [FromRoute]string userId,
            [FromQuery]int? productId = null,
            [FromQuery]string productName = null)
        {
            throw new NotImplementedException();
        }
    }
}
