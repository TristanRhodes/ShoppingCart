using NSubstitute;
using NUnit.Framework;
using ShoppingCart.Core.Commands;
using ShoppingCart.Core.Components;
using ShoppingCart.Core.Model;
using Shouldly;
using System;
using System.Collections.Generic;

namespace ShoppingCart.Core.Tests.Commands
{
    public abstract class AddItemToBasketCommandTests
    {
        protected IBasketRepository _basketRepository;
        protected IStockRepository _stockRepository;

        protected AddItemToBasketCommand _command;

        [SetUp]
        public void Setup()
        {
            _stockRepository = Substitute.For<IStockRepository>();
            _basketRepository = Substitute.For<IBasketRepository>();
            _command = new AddItemToBasketCommand(_stockRepository, _basketRepository);
        }

        public class CanAddItemToBasketCheck : AddItemToBasketCommandTests
        {
            [Test]
            public void ShouldReturnInvalidIdentifierIfTheIdentifierSuppliedIsInvalid()
            {
                var userId = "userId";
                var identifier = new ProductIdentifier(null, null);

                _command
                    .CanAddItemToBasketCheck(userId, identifier)
                    .ShouldBe(BasketOperationStatus.InvalidIdentifier);
            }

            [Test]
            public void ShouldReturnProductNotFoundWhenNoProductReturned()
            {
                var userId = "userId";
                var productId = 1;

                var identifier = new ProductIdentifier(productId, null);

                _stockRepository
                    .GetStockItem(productId)
                    .Returns((StockItem)null);

                _command
                    .CanAddItemToBasketCheck(userId, identifier)
                    .ShouldBe(BasketOperationStatus.ProductNotFound);
            }

            [Test]
            public void ShouldReturnInsufficientStockWhenNotEnoughStock()
            {
                var userId = "userId";
                var productId = 1;

                var identifier = new ProductIdentifier(productId, null);
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
                    .GetBasketItem(userId, productId)
                    .Returns(basketItem);

                _command
                    .CanAddItemToBasketCheck(userId, identifier)
                    .ShouldBe(BasketOperationStatus.InsufficientStock);
            }

            [Test]
            public void ShouldReturnOkWhenSufficientStock()
            {
                var userId = "userId";
                var productId = 1;

                var identifier = new ProductIdentifier(productId, null);
                var stockItem = new StockItem()
                {
                    Id = productId,
                    Stock = 1
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
                    .GetBasketItem(userId, productId)
                    .Returns(basketItem);

                _command
                    .CanAddItemToBasketCheck(userId, identifier)
                    .ShouldBe(BasketOperationStatus.Ok);
            }
        }

        public class AddItemToBasket : AddItemToBasketCommandTests
        {
            [Test]
            public void ShouldThrowInvalidIdentifierExceptionIfTheIdentifierSuppliedIsInvalid()
            {
                var userId = "userId";
                var identifier = new ProductIdentifier(null, null);

                Should.Throw<ApplicationException>(() => _command
                    .AddItemToBasket(userId, identifier));
            }

            [Test]
            public void ShouldThrowProductNotFoundExceptionIfProductNotFound()
            {
                var userId = "userId";
                var productId = 1;
                var identifier = new ProductIdentifier(productId, null);

                _stockRepository
                    .GetStockItem(productId)
                    .Returns((StockItem)null);

                Should.Throw<ApplicationException>(() => _command
                    .AddItemToBasket(userId, identifier));
            }

            [Test]
            public void ShouldAddItemToBasketAndReturnBasket()
            {
                var userId = "userId";
                var productId = 1;
                var identifier = new ProductIdentifier(productId, null);
                var stockItem = new StockItem() { Id = productId };
                var basket = new List<BasketItem>();

                _stockRepository
                    .GetStockItem(productId)
                    .Returns(stockItem);

                _basketRepository
                    .GetBasket(userId)
                    .Returns(basket);

                _command
                    .AddItemToBasket(userId, identifier)
                    .ShouldBe(basket);

                _basketRepository
                    .Received()
                    .AddItemToUserBasket(userId, productId);
            }
        }
    }
}
