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
        private IBasketManager _basketManager;

        public BasketController(
            IStockRepository stockRepository,
            IBasketRepository basketRepository,
            IBasketManager basketManager)
        {
            _stockRepository = stockRepository;
            _basketRepository = basketRepository;
            _basketManager = basketManager;
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
            var identifier = new ProductIdentifier(productId, productName);

            var actionStatus = _basketManager
                .CanAddItemToBasketCheck(userId, identifier);

            if (actionStatus == BasketOperationStatus.InvalidIdentifier)
                return BadRequest("Please supply a single value for productId or productName.");

            if (actionStatus == BasketOperationStatus.ProductNotFound)
                return NotFound("Product");

            if (actionStatus == BasketOperationStatus.InsufficientStock)
                return BadRequest("Not Enough Stock");
            
            var basket = _basketManager.AddItemToBasket(userId, identifier);
            return Json(basket);
        }

        [HttpPost("api/{userId}/basket/add")]
        public IActionResult BulkAddToBasket(
            [FromRoute]string userId,
            [FromBody]List<BasketItem> products)
        {
            var check = _basketManager.CanAddItemsToBasketCheck(userId, products);

            if (check.HasNotFoundProducts)
                return BadRequest("Products not found: " + string.Join(", ", check.ProductsNotFound));

            if (check.HasUnavailableProducts)
                return BadRequest("Not Enough Stock for item(s): " + string.Join(", ", check.ProductsNotAvailable));

            var basket = _basketManager.AddItemsToBasket(userId, products);
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

            var basket = _basketManager.RemoveItemFromBasket(userId, product.Id);
            return Json(basket);
        }

        [HttpPut("api/{userId}/basket/checkout")]
        public IActionResult CheckoutBasket(
            [FromRoute]string userId)
        {
            var results = _basketManager.CanCheckoutBasketCheck(userId);
            if (results.HasNotFoundProducts)
                return BadRequest("Products not found: " + string.Join(", ", results.ProductsNotFound));

            if (results.HasUnavailableProducts)
                return BadRequest("Not Enough Stock for item(s): " + string.Join(", ", results.ProductsNotAvailable));

            var invoice = _basketManager.CheckoutBasket(userId);
            return Json(invoice);
        }

        private static bool HasInvalidProductIdentifiers(int? productId, string productName)
        {
            return (!productId.HasValue && string.IsNullOrEmpty(productName))
                || (productId.HasValue && !string.IsNullOrEmpty(productName));
        }
    }
}
