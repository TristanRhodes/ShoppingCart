using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Core.Components;
using ShoppingCart.Core.Model;
using System;
using System.Collections.Generic;

namespace ShoppingCart.Core.Controllers
{
    public class BasketController : Controller
    {
        private IStockRepository _stockRepository;
        private IBasketRepository _basketRepository;
        private IBasketManager _coordinator;

        public BasketController(
            IStockRepository stockRepository,
            IBasketRepository basketRepository,
            IBasketManager coordinator)
        {
            _stockRepository = stockRepository;
            _basketRepository = basketRepository;
            _coordinator = coordinator;
        }

        [HttpGet("api/{userId}/basket")]
        public IActionResult GetBasket(
            [FromRoute]string userId)
        {
            return Json(_basketRepository.GetBasket(userId));
        }

        [HttpPut("api/{userId}/basket/add")]
        public IActionResult AddToBasket(
            [FromRoute]string userId,
            [FromQuery]int? productId = null,
            [FromQuery]string productName = null)
        {
            if (HasInvalidProductIdentifiers(productId, productName))
                return BadRequest("Please supply a single value for productId or productName.");

            var product = _stockRepository.GetStockItem(productId, productName);
            if (product == null)
                return NotFound("Product");

            if (!_coordinator.CanAddItemToBasket(userId, product.Id))
                return BadRequest("Not Enough Stock");
            
            var basket = _coordinator.AddItemToBasket(userId, product.Id);
            return Json(basket);
        }

        [HttpPost("api/{userId}/basket/add")]
        public IActionResult BulkAddToBasket(
            [FromRoute]string userId,
            [FromBody]List<BasketItem> products)
        {
            var check = _coordinator.CanAddItemsToBasketCheck(userId, products);

            if (check.HasNotFoundProducts)
                return BadRequest("Products not found: " + string.Join(", ", check.ProductsNotFound));

            if (check.HasUnavailableProducts)
                return BadRequest("Not Enough Stock for item(s): " + string.Join(", ", check.ProductsNotAvailable));

            var basket = _coordinator.AddItemsToBasket(userId, products);
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

            var product = _stockRepository.GetStockItem(productId, productName);
            if (product == null)
                return NotFound("Product");

            var basket = _coordinator.RemoveItemFromBasket(userId, product.Id);
            return Json(basket);
        }

        [HttpPut("api/{userId}/basket/checkout")]
        public IActionResult CheckoutBasket(
            [FromRoute]string userId)
        {
            var results = _coordinator.CanCheckoutBasketCheck(userId);
            if (results.HasNotFoundProducts)
                return BadRequest("Products not found: " + string.Join(", ", results.ProductsNotFound));

            if (results.HasUnavailableProducts)
                return BadRequest("Not Enough Stock for item(s): " + string.Join(", ", results.ProductsNotAvailable));

            var invoice = _coordinator.CheckoutBasket(userId);
            return Json(invoice);
        }

        private static bool HasInvalidProductIdentifiers(int? productId, string productName)
        {
            return (!productId.HasValue && string.IsNullOrEmpty(productName))
                || (productId.HasValue && !string.IsNullOrEmpty(productName));
        }
    }
}
