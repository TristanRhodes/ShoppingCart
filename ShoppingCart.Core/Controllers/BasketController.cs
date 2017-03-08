using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Core.Commands;
using ShoppingCart.Core.Components;
using ShoppingCart.Core.Model;
using System;
using System.Collections.Generic;

namespace ShoppingCart.Core.Controllers
{
    public class BasketController : Controller
    {
        private IBasketRepository _basketRepository;

        private IAddItemToBasketCommand _addCommand;
        private IBulkAddItemsToBasketCommand _bulkAddCommand;
        private IRemoveItemFromBasketCommand _removeCommand;
        private ICheckoutBasketCommand _checkoutCommand;

        public BasketController(
            IBasketRepository basketRepository,
            IAddItemToBasketCommand addCommand,
            IBulkAddItemsToBasketCommand bulkAddCommand,
            IRemoveItemFromBasketCommand removeCommand,
            ICheckoutBasketCommand checkoutCommand)
        {
            _basketRepository = basketRepository;
            _addCommand = addCommand;
            _bulkAddCommand = bulkAddCommand;
            _removeCommand = removeCommand;
            _checkoutCommand = checkoutCommand;
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

            var actionStatus = _addCommand
                .CanAddItemToBasketCheck(userId, identifier);

            if (actionStatus == BasketOperationStatus.InvalidIdentifier)
                return BadRequest("Please supply a single value for productId or productName.");

            if (actionStatus == BasketOperationStatus.ProductNotFound)
                return NotFound("Product");

            if (actionStatus == BasketOperationStatus.InsufficientStock)
                return BadRequest("Not Enough Stock");
            
            var basket = _addCommand.AddItemToBasket(userId, identifier);
            return Json(basket);
        }

        [HttpPost("api/{userId}/basket/add")]
        public IActionResult BulkAddToBasket(
            [FromRoute]string userId,
            [FromBody]List<BasketItem> products)
        {
            var check = _bulkAddCommand
                .CanAddItemsToBasketCheck(userId, products);

            if (check.HasNotFoundProducts)
                return BadRequest("Products not found: " + string.Join(", ", check.ProductsNotFound));

            if (check.HasUnavailableProducts)
                return BadRequest("Not Enough Stock for item(s): " + string.Join(", ", check.ProductsNotAvailable));

            var basket = _bulkAddCommand
                .AddItemsToBasket(userId, products);

            return Json(basket);
        }

        [HttpPut("api/{userId}/basket/remove")]
        public IActionResult RemoveFromBasket(
            [FromRoute]string userId,
            [FromQuery]int? productId = null,
            [FromQuery]string productName = null)
        {
            var identifier = new ProductIdentifier(productId, productName);

            var actionStatus = _removeCommand
                .CanRemoveItemFromBasketCheck(userId, identifier);

            if (actionStatus == BasketOperationStatus.InvalidIdentifier)
                return BadRequest("Please supply a single value for productId or productName.");

            if (actionStatus == BasketOperationStatus.ProductNotFound)
                return NotFound("Product");

            if (actionStatus == BasketOperationStatus.NotInBasket)
                return BadRequest("Not in basket");

            var basket = _removeCommand
                .RemoveItemFromBasket(userId, identifier);

            return Json(basket);
        }

        [HttpPut("api/{userId}/basket/checkout")]
        public IActionResult CheckoutBasket(
            [FromRoute]string userId)
        {
            var results = _checkoutCommand
                .CanCheckoutBasketCheck(userId);

            if (results.HasNotFoundProducts)
                return BadRequest("Products not found: " + string.Join(", ", results.ProductsNotFound));

            if (results.HasUnavailableProducts)
                return BadRequest("Not Enough Stock for item(s): " + string.Join(", ", results.ProductsNotAvailable));

            var invoice = _checkoutCommand
                .CheckoutBasket(userId);

            return Json(invoice);
        }
    }
}
