using NSubstitute;
using NUnit.Framework;
using ShoppingCart.Core.Components;
using ShoppingCart.Core.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Core.Model;
using ShoppingCart.Core.Commands;

namespace ShoppingCart.Core.Tests.Controllers
{
    public class BasketControllerTests
    {
        protected IBasketRepository _basketRepository;

        protected IAddItemToBasketCommand _addCommand;
        protected IBulkAddItemsToBasketCommand _bulkAddCommand;
        protected IRemoveItemFromBasketCommand _removeCommand;
        protected ICheckoutBasketCommand _checkoutCommand;

        protected BasketController _controller;

        [SetUp]
        public void Setup()
        {
            _basketRepository = Substitute.For<IBasketRepository>();
            _addCommand = Substitute.For<IAddItemToBasketCommand>();
            _bulkAddCommand = Substitute.For<IBulkAddItemsToBasketCommand>();
            _removeCommand = Substitute.For<IRemoveItemFromBasketCommand>();
            _checkoutCommand = Substitute.For<ICheckoutBasketCommand>();

            _controller = new BasketController(
                _basketRepository, 
                _addCommand, 
                _bulkAddCommand, 
                _removeCommand, 
                _checkoutCommand);
        }

        public class AddToBasket : BasketControllerTests
        {
            [Test]
            public void ShouldReturnBadRequestWhenProductIdentifierIsInvalid()
            {
                var userName = "user";

                _addCommand
                    .CanAddItemToBasketCheck(userName, Arg.Any<ProductIdentifier>())
                    .Returns(BasketOperationStatus.InvalidIdentifier);

                _controller
                   .AddToBasket(userName, null, null)
                   .ShouldBeOfType<BadRequestObjectResult>();
            }

            [Test]
            public void ShouldReturnNotFoundWhenNoProductExists()
            {
                var userName = "user";

                _addCommand
                    .CanAddItemToBasketCheck(userName, Arg.Any<ProductIdentifier>())
                    .Returns(BasketOperationStatus.ProductNotFound);

                _controller
                    .AddToBasket(userName, null, null)
                    .ShouldBeOfType<NotFoundObjectResult>();
            }

            [Test]
            public void ShouldReturnBadRequestWhenNotEnoughStockToSupportAdding()
            {
                var userName = "user";
                var productId = 1;

                _addCommand
                    .CanAddItemToBasketCheck(userName, Arg.Any<ProductIdentifier>())
                    .Returns(BasketOperationStatus.InsufficientStock);

                _controller
                    .AddToBasket(userName, productId)
                    .ShouldBeOfType<BadRequestObjectResult>();
            }

            [Test]
            public void ShouldAddToBasketWhenEnoughStock()
            {
                var userName = "user";
                var productId = 1;

                _addCommand
                    .CanAddItemToBasketCheck(userName, Arg.Any<ProductIdentifier>())
                    .Returns(BasketOperationStatus.Ok);

                _controller
                    .AddToBasket(userName, productId)
                    .ShouldBeOfType<JsonResult>();

                _addCommand
                    .Received()
                    .AddItemToBasket(userName, Arg.Any<ProductIdentifier>());
            }
        }

        public class RemoveFromBasket : BasketControllerTests
        {
            [Test]
            public void ShouldReturnBadRequestWhenBothProductIdAndProductNameAreNotSupplied()
            {
                var userId = "user";
                var productId = 1;

                _removeCommand
                    .CanRemoveItemFromBasketCheck(userId, Arg.Any<ProductIdentifier>())
                    .Returns(BasketOperationStatus.InvalidIdentifier);

                _controller
                   .RemoveFromBasket(userId, productId, null)
                   .ShouldBeOfType<BadRequestObjectResult>();
            }

            [Test]
            public void ShouldReturnNotFoundWhenNoProductExists()
            {
                var userId = "user";
                var productId = 1;

                _removeCommand
                    .CanRemoveItemFromBasketCheck(userId, Arg.Any<ProductIdentifier>())
                    .Returns(BasketOperationStatus.ProductNotFound);

                _controller
                    .RemoveFromBasket(userId, productId)
                    .ShouldBeOfType<NotFoundObjectResult>();
            }

            [Test]
            public void ShouldReturnNotInBasketWhenBasketItemIsEmpty()
            {
                var userId = "user";
                var productId = 1;

                _removeCommand
                    .CanRemoveItemFromBasketCheck(userId, Arg.Any<ProductIdentifier>())
                    .Returns(BasketOperationStatus.NotInBasket);

                _controller
                    .RemoveFromBasket(userId, productId)
                    .ShouldBeOfType<BadRequestObjectResult>();
            }

            [Test]
            public void ShouldRemoveFromBasketWhenLastItemRemoved()
            {
                var userName = "user";
                var productId = 1;

                _controller
                    .RemoveFromBasket(userName, productId)
                    .ShouldBeOfType<JsonResult>();

                _removeCommand
                    .Received()
                    .RemoveItemFromBasket(userName, Arg.Any<ProductIdentifier>());
            }
        }

        public class BulkAddToBasket : BasketControllerTests
        {
            [Test]
            public void ShouldReturnBadRequestWhenItemsNotFound()
            {
                var userName = "user";
                var productsToAdd = new List<BasketItem>();

                var check = new AvailabilityCheckResults();
                check.Result = AvailabilityCheckStatus.ProductsNotFound;
                check.ProductsNotFound.Add(1);

                _bulkAddCommand
                    .CanAddItemsToBasketCheck(userName, productsToAdd)
                    .Returns(check);

                _controller
                    .BulkAddToBasket(userName, productsToAdd)
                    .ShouldBeOfType<BadRequestObjectResult>()
                    .Value.ShouldBeOfType<string>()
                    .ShouldStartWith("Products not found: ");
            }

            [Test]
            public void ShouldReturnBadRequestWhenItemsWithInsufficientStock()
            {
                var userName = "user";
                var productsToAdd = new List<BasketItem>();

                var check = new AvailabilityCheckResults();
                check.Result = AvailabilityCheckStatus.InsufficientStock;
                check.ProductsNotAvailable.Add("product");

                _bulkAddCommand
                    .CanAddItemsToBasketCheck(userName, productsToAdd)
                    .Returns(check);

                _controller
                    .BulkAddToBasket(userName, productsToAdd)
                    .ShouldBeOfType<BadRequestObjectResult>()
                    .Value.ShouldBeOfType<string>()
                    .ShouldStartWith("Not Enough Stock for item(s): ");
            }

            [Test]
            public void ShouldAddToBasketRequestWhenCanAddItemsToBasket()
            {
                var userName = "user";
                var productsToAdd = new List<BasketItem>();

                var check = new AvailabilityCheckResults();

                var basket = new List<BasketItem>();

                _bulkAddCommand
                    .CanAddItemsToBasketCheck(userName, productsToAdd)
                    .Returns(check);

                _bulkAddCommand
                    .AddItemsToBasket(userName, productsToAdd)
                    .Returns(basket);

                _controller
                    .BulkAddToBasket(userName, productsToAdd)
                    .ShouldBeOfType<JsonResult>()
                    .Value.ShouldBeOfType<List<BasketItem>>();

                _bulkAddCommand
                    .Received()
                    .AddItemsToBasket(userName, productsToAdd);
            }
        }

        public class BasketCheckout : BasketControllerTests
        {
            [Test]
            public void ShouldReturnBadRequestWhenItemsNotFound()
            {
                var userName = "user";
                var productsToAdd = new List<BasketItem>();

                var check = new AvailabilityCheckResults();
                check.Result = AvailabilityCheckStatus.ProductsNotFound;
                check.ProductsNotFound.Add(1);

                _checkoutCommand
                    .CanCheckoutBasketCheck(userName)
                    .Returns(check);

                _controller
                    .CheckoutBasket(userName)
                    .ShouldBeOfType<BadRequestObjectResult>()
                    .Value.ShouldBeOfType<string>()
                    .ShouldStartWith("Products not found: ");
            }

            [Test]
            public void ShouldReturnBadRequestWhenItemsWithInsufficientStock()
            {
                var userName = "user";
                var productsToAdd = new List<BasketItem>();

                var check = new AvailabilityCheckResults();
                check.Result = AvailabilityCheckStatus.InsufficientStock;
                check.ProductsNotAvailable.Add("product");

                _checkoutCommand
                    .CanCheckoutBasketCheck(userName)
                    .Returns(check);

                _controller
                    .CheckoutBasket(userName)
                    .ShouldBeOfType<BadRequestObjectResult>()
                    .Value.ShouldBeOfType<string>()
                    .ShouldStartWith("Not Enough Stock for item(s): ");
            }

            [Test]
            public void ShouldReturnInvoiceAndDeductStockWhenSuccesful()
            {
                var userName = "user";
                var productsToAdd = new List<BasketItem>();

                var check = new AvailabilityCheckResults();

                var invoice = new Invoice();

                _checkoutCommand
                    .CanCheckoutBasketCheck(userName)
                    .Returns(check);

                _checkoutCommand
                    .CheckoutBasket(userName)
                    .Returns(invoice);

                _controller
                    .CheckoutBasket(userName)
                    .ShouldBeOfType<JsonResult>()
                    .Value.ShouldBeOfType<Invoice>()
                    .ShouldBe(invoice);
            }
        }
    }
}
