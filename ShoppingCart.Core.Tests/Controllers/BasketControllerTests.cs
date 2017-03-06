﻿using NSubstitute;
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

namespace ShoppingCart.Core.Tests.Controllers
{
    public class BasketControllerTests
    {
        public abstract class BasketTestBase
        {
            protected IBasketRepository _basketRepository;
            protected IStockRepository _stockRepository;
            protected IBasketManager _basketManager;

            protected BasketController _controller;

            [SetUp]
            public void Setup()
            {
                _stockRepository = Substitute.For<IStockRepository>();
                _basketRepository = Substitute.For<IBasketRepository>();
                _basketManager = Substitute.For<IBasketManager>();

                _controller = new BasketController(_stockRepository, _basketRepository, _basketManager);
            }
        }

        public class AddToBasket : BasketTestBase
        {
            [Test]
            public void ShouldReturnBadRequestWhenBothProductIdAndProductNameAreSupplied()
            {
                _controller
                    .AddToBasket(string.Empty, 1, "1")
                    .ShouldBeOfType<BadRequestObjectResult>();
            }

            [Test]
            public void ShouldReturnBadRequestWhenBothProductIdAndProductNameAreNotSupplied()
            {
                _controller
                   .AddToBasket(string.Empty, null, null)
                   .ShouldBeOfType<BadRequestObjectResult>();
            }

            [Test]
            public void ShouldReturnNotFoundWhenNoProductExists()
            {
                var userName = "user";
                var productId = 1;

                _controller
                    .AddToBasket(userName, productId)
                    .ShouldBeOfType<NotFoundObjectResult>();
            }

            [Test]
            public void ShouldReturnBadRequestWhenNotEnoughStockToSupportAdding()
            {
                var userName = "user";
                var productId = 1;

                var stockItem = new StockItem()
                {
                    Id = productId,
                    Stock = 0
                };

                _stockRepository
                    .GetStockItem(productId)
                    .Returns(stockItem);

                _controller
                    .AddToBasket(userName, productId)
                    .ShouldBeOfType<BadRequestObjectResult>();
            }

            [Test]
            public void ShouldAddToBasketWhenEnoughStock()
            {
                var userName = "user";
                var productId = 1;

                var stockItem = new StockItem()
                {
                    Id = productId,
                    Stock = 2
                };

                _stockRepository
                    .GetStockItem(productId)
                    .Returns(stockItem);

                _basketManager
                    .CanAddItemToBasket(userName, productId)
                    .Returns(true);

                _controller
                    .AddToBasket(userName, productId)
                    .ShouldBeOfType<JsonResult>();

                _basketManager
                    .Received()
                    .AddItemToBasket(userName, productId);
            }
        }

        public class RemoveFromBasket : BasketTestBase
        {
            [Test]
            public void ShouldReturnBadRequestWhenBothProductIdAndProductNameAreSupplied()
            {
                _controller
                    .RemoveFromBasket(string.Empty, 1, "1")
                    .ShouldBeOfType<BadRequestObjectResult>();
            }

            [Test]
            public void ShouldReturnBadRequestWhenBothProductIdAndProductNameAreNotSupplied()
            {
                _controller
                   .RemoveFromBasket(string.Empty, null, null)
                   .ShouldBeOfType<BadRequestObjectResult>();
            }

            [Test]
            public void ShouldReturnNotFoundWhenNoProductExists()
            {
                var userName = "user";
                var productId = 1;

                _controller
                    .RemoveFromBasket(userName, productId)
                    .ShouldBeOfType<NotFoundObjectResult>();
            }

            [Test]
            public void ShouldRemoveFromBasketWhenLastItemRemoved()
            {
                var userName = "user";
                var productId = 1;

                var stockItem = new StockItem()
                {
                    Id = productId,
                    Stock = 2
                };

                _stockRepository
                    .GetStockItem(productId)
                    .Returns(stockItem);

                _controller
                    .RemoveFromBasket(userName, productId)
                    .ShouldBeOfType<JsonResult>();

                _basketManager
                    .Received()
                    .RemoveItemFromBasket(userName, productId);
            }
        }

        public class BulkAddToBasket : BasketTestBase
        {
            [Test]
            public void ShouldReturnBadRequestWhenItemsNotFound()
            {
                var userName = "user";
                var productsToAdd = new List<BasketItem>();

                var check = new AvailabilityCheckResults();
                check.Available = false;
                check.ProductsNotFound.Add(1);

                _basketManager
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
                check.Available = false;
                check.ProductsNotAvailable.Add("product");

                _basketManager
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
                check.Available = false;

                var basket = new List<BasketItem>();

                _basketManager
                    .CanAddItemsToBasketCheck(userName, productsToAdd)
                    .Returns(check);

                _basketManager
                    .AddItemsToBasket(userName, productsToAdd)
                    .Returns(basket);

                _controller
                    .BulkAddToBasket(userName, productsToAdd)
                    .ShouldBeOfType<JsonResult>()
                    .Value.ShouldBeOfType<List<BasketItem>>();

                _basketManager
                    .Received()
                    .AddItemsToBasket(userName, productsToAdd);
            }
        }

        public class BasketCheckout : BasketTestBase
        {
            [Test]
            public void ShouldReturnBadRequestWhenItemsNotFound()
            {
                var userName = "user";
                var productsToAdd = new List<BasketItem>();

                var check = new AvailabilityCheckResults();
                check.Available = false;
                check.ProductsNotFound.Add(1);

                _basketManager
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
                check.Available = false;
                check.ProductsNotAvailable.Add("product");

                _basketManager
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
                check.Available = false;

                var invoice = new Invoice();

                _basketManager
                    .CanCheckoutBasketCheck(userName)
                    .Returns(check);

                _basketManager
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
