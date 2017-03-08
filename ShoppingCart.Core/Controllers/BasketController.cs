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

            var response = GetFailureResponseForAction(actionStatus);
            if (response != null)
                return response;

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

            if (check.Result == AvailabilityCheckStatus.ProductsNotFound)
                return BadRequest("Products not found: " + string.Join(", ", check.ProductsNotFound));

            if (check.Result == AvailabilityCheckStatus.InsufficientStock)
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

            var response = GetFailureResponseForAction(actionStatus);
            if (response != null)
                return response;

            var basket = _removeCommand
                .RemoveItemFromBasket(userId, identifier);

            return Json(basket);
        }

        [HttpPut("api/{userId}/basket/checkout")]
        public IActionResult CheckoutBasket(
            [FromRoute]string userId)
        {
            var check = _checkoutCommand
                .CanCheckoutBasketCheck(userId);

            if (check.Result == AvailabilityCheckStatus.ProductsNotFound)
                return BadRequest("Products not found: " + string.Join(", ", check.ProductsNotFound));

            if (check.Result == AvailabilityCheckStatus.InsufficientStock)
                return BadRequest("Not Enough Stock for item(s): " + string.Join(", ", check.ProductsNotAvailable));

            var invoice = _checkoutCommand
                .CheckoutBasket(userId);

            return Json(invoice);
        }
         
        private IActionResult GetFailureResponseForAction(BasketOperationStatus actionStatus)
        {
            switch (actionStatus)
            {
                case BasketOperationStatus.Ok:
                    return null;
                case BasketOperationStatus.ProductNotFound:
                    return NotFound("Product");
                case BasketOperationStatus.InsufficientStock:
                    return BadRequest("Not Enough Stock");
                case BasketOperationStatus.InvalidIdentifier:
                    return BadRequest("Please supply a single value for productId or productName.");
                case BasketOperationStatus.NotInBasket:
                    return BadRequest("Not in basket");
                default:
                    throw new ApplicationException("Unhandled enumeration value: " + actionStatus);
            }
        }
    }
}
