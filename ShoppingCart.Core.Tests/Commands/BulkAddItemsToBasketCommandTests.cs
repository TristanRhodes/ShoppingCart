using NSubstitute;
using NUnit.Framework;
using ShoppingCart.Core.Commands;
using ShoppingCart.Core.Components;
using ShoppingCart.Core.Model;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCart.Core.Tests.Commands
{
    public abstract class BulkAddItemsToBasketCommandTests
    {
        protected IBasketRepository _basketRepository;
        protected IStockRepository _stockRepository;

        protected BulkAddItemsToBasketCommand _command;

        [SetUp]
        public void Setup()
        {
            _stockRepository = Substitute.For<IStockRepository>();
            _basketRepository = Substitute.For<IBasketRepository>();
            _command = new BulkAddItemsToBasketCommand(_stockRepository, _basketRepository);
        }

        public class CanAddItemsToBasketCheck : BulkAddItemsToBasketCommandTests
        {
            [Test]
            public void ShouldReturnUnavailableWhenProductsNotFound()
            {
                var userId = "userId";
                var productId = 1;
                var basket = new List<BasketItem>()
                {
                    new BasketItem()
                    {
                        ProductId = productId,
                        ItemCount = 1
                    }
                };

                _stockRepository
                    .GetStockItem(productId)
                    .Returns((StockItem)null);

                var check = _command
                    .CanAddItemsToBasketCheck(userId, basket);

                check.Available.ShouldBeFalse();
                check.ProductsNotFound.Count.ShouldBe(1);
                check.ProductsNotFound.Single().ShouldBe(productId);
                check.ProductsNotAvailable.ShouldBeEmpty();
            }

            [Test]
            public void ShouldReturnUnavailableWhenProductsHaveInsufficientStock()
            {
                var userId = "userId";
                var productId = 1;
                var productName = "name";
                var stockItem = new StockItem()
                {
                    Id = productId,
                    Name = productName,
                    Stock = 1
                };
                var basketItem = new BasketItem()
                {
                    ProductId = productId,
                    ItemCount = 1
                };

                var basket = new List<BasketItem>()
                {
                    basketItem
                };

                _stockRepository
                    .GetStockItem(productId)
                    .Returns(stockItem);

                _basketRepository
                    .GetBasketItem(userId, productId)
                    .Returns(basketItem);

                var check = _command
                    .CanAddItemsToBasketCheck(userId, basket);

                check.Available.ShouldBeFalse();
                check.ProductsNotAvailable.Count.ShouldBe(1);
                check.ProductsNotAvailable.Single().ShouldBe(productName);
                check.ProductsNotFound.ShouldBeEmpty();
            }

            [Test]
            public void ShouldReturnOkWhenProductIsInStock()
            {
                var userId = "userId";
                var productId = 1;
                var productName = "name";
                var stockItem = new StockItem()
                {
                    Id = productId,
                    Name = productName,
                    Stock = 2
                };
                var basketItem = new BasketItem()
                {
                    ProductId = productId,
                    ItemCount = 1
                };
                var basket = new List<BasketItem>()
                {
                    basketItem
                };

                _stockRepository
                    .GetStockItem(productId)
                    .Returns(stockItem);

                _basketRepository
                    .GetBasketItem(userId, productId)
                    .Returns(basketItem);

                var check = _command
                    .CanAddItemsToBasketCheck(userId, basket);

                check.Available.ShouldBeTrue();
                check.ProductsNotAvailable.ShouldBeEmpty();
                check.ProductsNotFound.ShouldBeEmpty();
            }
        }

        public class AddItemsToBasket : BulkAddItemsToBasketCommandTests
        {
            [Test]
            public void ShouldAddItemsToBasketAndReturnBasket()
            {
                var userId = "userId";
                var productId = 1;

                var basketItem = new BasketItem()
                {
                    ProductId = productId,
                    ItemCount = 2
                };
                var basket = new List<BasketItem>()
                {
                    basketItem
                };

                _basketRepository
                    .GetBasket(userId)
                    .Returns(basket);

                _command
                    .AddItemsToBasket(userId, basket)
                    .ShouldBe(basket);

                _basketRepository
                    .Received()
                    .AddItemToUserBasket(userId, basketItem.ProductId, basketItem.ItemCount);
            }
        }
    }
}
