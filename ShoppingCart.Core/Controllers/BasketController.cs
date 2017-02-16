using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Core.Components;
using ShoppingCart.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Core.Controllers
{
    public class BasketController : Controller
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
            [FromRoute]string userId)
        {
            return Json(_basketManager.GetBasket(userId));
        }

        [HttpPut("api/{userId}/basket/add")]
        public IActionResult AddToBasket(
            [FromRoute]string userId,
            [FromQuery]int? productId = null,
            [FromQuery]string productName = null)
        {
            if (HasInvalidProductIdentifiers(productId, productName))
                return BadRequest("Please supply a single value for productId or productName.");

            var product = ResolveProduct(productId, productName);
            if (product == null)
                return NotFound("Product");

            var basketItem = _basketManager.GetBasketItem(userId, product.Id);
            if (!product.HasSufficientStockFor(basketItem.ItemCount + 1))
                return BadRequest("Not Enough Stock");
            
            _basketManager.AddItemToUserBasket(userId, product.Id);

            var basket = _basketManager.GetBasket(userId);
            return Json(basket);
        }

        [HttpPost("api/{userId}/basket/add")]
        public IActionResult BulkAddToBasket(
            [FromRoute]string userId,
            [FromBody]List<BasketItem> products)
        {
            // Validate
            foreach(var addToBasketItem in products)
            {
                var product = _stockManager.GetStockItem(addToBasketItem.ProductId);
                var basketItem = _basketManager.GetBasketItem(userId, addToBasketItem.ProductId);

                if (!product.HasSufficientStockFor(basketItem.ItemCount + addToBasketItem.ItemCount))
                    return BadRequest("Not Enough Stock for item: " + product.Name);
            }

            // Apply
            foreach(var item in products)
            {
                _basketManager.AddItemToUserBasket(userId, item.ProductId, item.ItemCount);
            }

            var basket = _basketManager.GetBasket(userId);
            return Json(basket);
        }
        
        [HttpPut("api/{userId}/basket/remove")]
        public IActionResult RemoveFromBasket(
            [FromRoute]string userId,
            [FromQuery]int? productId = null,
            [FromQuery]string productName = null)
        {
            if (HasInvalidProductIdentifiers(productId, productName))
                return BadRequest("Please supply a single value for productId or productName.");

            var product = ResolveProduct(productId, productName);
            if (product == null)
                return NotFound("Product");

            _basketManager.RemoveItemFromUserBasket(userId, product.Id);

            var basket = _basketManager.GetBasket(userId);
            return Json(basket);
        }
        
        private StockItem ResolveProduct(int? productId, string productName)
        {
            if (productId.HasValue)
                return _stockManager.GetStockItem(productId.Value);
            else
                return _stockManager.GetStockItem(productName);
        }

        private static bool HasInvalidProductIdentifiers(int? productId, string productName)
        {
            return (!productId.HasValue && string.IsNullOrEmpty(productName))
                || (productId.HasValue && !string.IsNullOrEmpty(productName));
        }
    }
}
