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
            protected IBasketRepository _basketRepository;
            protected IStockRepository _stockRepository;

            protected BasketController _controller;
            protected BasketManager _coordinator;

            [SetUp]
            public void Setup()
            {
                _stockRepository = Substitute.For<IStockRepository>();
                _basketRepository = Substitute.For<IBasketRepository>();
                _coordinator = new BasketManager(_stockRepository, _basketRepository);

                _controller = new BasketController(_stockRepository, _basketRepository, _coordinator);
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

                _stockRepository
                    .GetStockItem(productId)
                    .Returns(stockItem);

                _basketRepository
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

                _stockRepository
                    .GetStockItem(productId)
                    .Returns(stockItem);

                _basketRepository
                    .GetBasketItem(userName, productId)
                    .Returns(basketItem);

                _basketRepository
                    .GetBasket(userName)
                    .Returns(new List<BasketItem>());

                _controller
                    .AddToBasket(userName, productId)
                    .ShouldBeOfType<JsonResult>();

                _basketRepository
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
            public void ShouldRemoveFromBasketWhenLastItemRemoved()
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

                _stockRepository
                    .GetStockItem(productId)
                    .Returns(stockItem);

                _basketRepository
                    .GetBasketItem(userName, productId)
                    .Returns(basketItem);

                _basketRepository
                    .GetBasket(userName)
                    .Returns(new List<BasketItem>());

                _controller
                    .RemoveFromBasket(userName, productId)
                    .ShouldBeOfType<JsonResult>();

                _basketRepository
                    .Received()
                    .RemoveItemFromUserBasket(userName, productId);
            }
        }

        public class BulkAddToBasket : BasketTestBase
        {
            [Test]
            public void ShouldReturnBadRequestWhenItemsWithInsufficientStock()
            {
                var userName = "user";
                var productId = 1;

                var stockItem = new StockItem()
                {
                    Id = productId,
                    Stock = 1
                };

                var basketItem = new BasketItem()
                {
                    ProductId = productId,
                    ItemCount = 1
                };

                var productsToAdd = new List<BasketItem>()
                {
                    new BasketItem() { ProductId = 1, ItemCount = 2 }
                };

                _stockRepository
                    .GetStockItem(productId)
                    .Returns(stockItem);

                _basketRepository
                    .GetBasketItem(userName, productId)
                    .Returns(basketItem);

                _controller
                    .BulkAddToBasket(userName, productsToAdd)
                    .ShouldBeOfType<BadRequestObjectResult>();
            }

            [Test]
            public void ShouldAddToBasketRequestWhenItemsInStock()
            {
                var userName = "user";
                var productId1 = 1;
                var productId2 = 2;
                var itemsToAdd = 2;

                var stockItem1 = new StockItem()
                {
                    Id = productId1,
                    Stock = 3
                };

                var stockItem2 = new StockItem()
                {
                    Id = productId2,
                    Stock = 3
                };

                var basketItem1 = new BasketItem()
                {
                    ProductId = productId1,
                    ItemCount = 1
                };

                var basketItem2 = new BasketItem()
                {
                    ProductId = productId2,
                    ItemCount = 1
                };

                var productsToAdd = new List<BasketItem>()
                {
                    new BasketItem() { ProductId = productId1, ItemCount = itemsToAdd },
                    new BasketItem() { ProductId = productId2, ItemCount = itemsToAdd }
                };

                _stockRepository
                    .GetStockItem(productId1)
                    .Returns(stockItem1);

                _stockRepository
                    .GetStockItem(productId2)
                    .Returns(stockItem2);

                _basketRepository
                    .GetBasketItem(userName, productId1)
                    .Returns(basketItem1);

                _basketRepository
                    .GetBasketItem(userName, productId2)
                    .Returns(basketItem2);

                _controller
                    .BulkAddToBasket(userName, productsToAdd)
                    .ShouldBeOfType<JsonResult>();

                _basketRepository
                    .Received()
                    .AddItemToUserBasket(userName, productId1, itemsToAdd);

                _basketRepository
                    .Received()
                    .AddItemToUserBasket(userName, productId2, itemsToAdd);
            }
        }

        public class BasketCheckout : BasketTestBase
        {
            [Test]
            public void ShouldReturnBadRequestWhenProductNotFound()
            {
                var userName = "user";
                var productId = 1;

                var basketItems = new List<BasketItem>()
                {
                    new BasketItem() { ProductId = productId, ItemCount = 1}
                };

                _basketRepository
                    .GetBasket(userName)
                    .Returns(basketItems);

                _controller
                    .CheckoutBasket(userName)
                    .ShouldBeOfType<BadRequestObjectResult>()
                    .Value.ShouldBeOfType<string>()
                    .ShouldBe("Products not found: " + productId);
            }

            [Test]
            public void ShouldReturnBadRequestWhenInsufficientStock()
            {
                var productId = 1;
                var productName = "productName";
                var userName = "user";

                var stockItem = new StockItem()
                {
                    Id = productId,
                    Stock = 0,
                    Name = productName
                };

                var basketItems = new List<BasketItem>()
                {
                    new BasketItem() { ProductId = productId, ItemCount = 1}
                };

                _basketRepository
                    .GetBasket(userName)
                    .Returns(basketItems);

                _stockRepository
                    .GetStockItem(productId)
                    .Returns(stockItem);

                _controller
                    .CheckoutBasket(userName)
                    .ShouldBeOfType<BadRequestObjectResult>()
                    .Value.ShouldBeOfType<string>()
                    .ShouldBe("Not Enough Stock for item(s): " + productName);
            }
            
            [Test]
            public void ShouldReturnInvoiceAndDeductStockWhenSuccesful()
            {
                var productId = 1;
                var stock = 2;
                var price = 6.99M;
                var productName = "productName";
                var userName = "user";

                var stockItem = new StockItem()
                {
                    Id = productId,
                    Stock = stock,
                    Name = productName,
                    Price = price
                };

                var basketItems = new List<BasketItem>()
                {
                    new BasketItem() { ProductId = productId, ItemCount = stock}
                };

                _basketRepository
                    .GetBasket(userName)
                    .Returns(basketItems);

                _stockRepository
                    .GetStockItem(productId)
                    .Returns(stockItem);
                _basketRepository
                    .GetBasket(userName)
                    .Returns(basketItems);

                var invoice = _controller
                    .CheckoutBasket(userName)
                    .ShouldBeOfType<JsonResult>()
                    .Value.ShouldBeOfType<Invoice>();

                invoice.Total.ShouldBe(stock * price);
                invoice.User.ShouldBe(userName);
                invoice.Items.Count.ShouldBe(1);
            }
        }
    }
}
