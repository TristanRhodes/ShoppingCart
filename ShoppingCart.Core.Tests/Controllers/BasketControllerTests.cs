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

namespace ShoppingCart.Core.Tests.Controllers
{
    public class BasketControllerTests
    {
        public abstract class BasketTestBase
        {
            protected IBasketManager _basketManager;
            protected IStockManager _stockManager;

            protected BasketController _controller;

            [SetUp]
            public void Setup()
            {
                _stockManager = Substitute.For<IStockManager>();
                _basketManager = Substitute.For<IBasketManager>();

                _controller = new BasketController(_stockManager, _basketManager);
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

                var basketItem = new BasketItem()
                {
                    ProductId = productId,
                    ItemCount = 0
                };

                _stockManager
                    .GetStockItem(productId)
                    .Returns(stockItem);

                _basketManager
                    .GetBasketItem(userName, productId)
                    .Returns(basketItem);

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

                var basketItem = new BasketItem()
                {
                    ProductId = productId,
                    ItemCount = 1
                };

                _stockManager
                    .GetStockItem(productId)
                    .Returns(stockItem);

                _basketManager
                    .GetBasketItem(userName, productId)
                    .Returns(basketItem);

                _basketManager
                    .GetBasket(userName)
                    .Returns(new List<BasketItem>());

                _controller
                    .AddToBasket(userName, productId)
                    .ShouldBeOfType<JsonResult>();

                _basketManager
                    .Received()
                    .AddItemToUserBasket(userName, productId);
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
            public void ShouldRemoveFromBasket()
            {
                var userName = "user";
                var productId = 1;

                var stockItem = new StockItem()
                {
                    Id = productId,
                    Stock = 2
                };

                var basketItem = new BasketItem()
                {
                    ProductId = productId,
                    ItemCount = 1
                };

                _stockManager
                    .GetStockItem(productId)
                    .Returns(stockItem);

                _basketManager
                    .GetBasketItem(userName, productId)
                    .Returns(basketItem);

                _basketManager
                    .GetBasket(userName)
                    .Returns(new List<BasketItem>());

                _controller
                    .RemoveFromBasket(userName, productId)
                    .ShouldBeOfType<JsonResult>();

                _basketManager
                    .Received()
                    .RemoveItemFromUserBasket(userName, productId);
            }
        }
    }
}
