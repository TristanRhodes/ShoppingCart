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

    public abstract class RemoveItemFromBasketCommandTests
    {
        protected IBasketRepository _basketRepository;
        protected IStockRepository _stockRepository;

        protected RemoveItemFromBasketCommand _command;

        [SetUp]
        public void Setup()
        {
            _stockRepository = Substitute.For<IStockRepository>();
            _basketRepository = Substitute.For<IBasketRepository>();
            _command = new RemoveItemFromBasketCommand(_stockRepository, _basketRepository);
        }

        public class CanRemoveItemFromBasketCheck : RemoveItemFromBasketCommandTests
        {
            [Test]
            public void ShouldReturnInvalidIdentifierIfTheIdentifierSuppliedIsInvalid()
            {
                var userId = "userId";
                var identifier = new ProductIdentifier(null, null);

                _command
                    .CanRemoveItemFromBasketCheck(userId, identifier)
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
                    .CanRemoveItemFromBasketCheck(userId, identifier)
                    .ShouldBe(BasketOperationStatus.ProductNotFound);
            }

            [Test]
            public void ShouldReturnNotInBasketWhenNotEnoughStock()
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
                    .CanRemoveItemFromBasketCheck(userId, identifier)
                    .ShouldBe(BasketOperationStatus.NotInBasket);
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
                    ItemCount = 1
                };

                _stockRepository
                    .GetStockItem(productId)
                    .Returns(stockItem);

                _basketRepository
                    .GetBasketItem(userId, productId)
                    .Returns(basketItem);

                _command
                    .CanRemoveItemFromBasketCheck(userId, identifier)
                    .ShouldBe(BasketOperationStatus.Ok);
            }
        }

        public class RemoveItemFromBasket : RemoveItemFromBasketCommandTests
        {
            [Test]
            public void ShouldThrowInvalidIdentifierExceptionIfTheIdentifierSuppliedIsInvalid()
            {
                var userId = "userId";
                var identifier = new ProductIdentifier(null, null);

                Should.Throw<ApplicationException>(() => _command
                    .RemoveItemFromBasket(userId, identifier));
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
                    .RemoveItemFromBasket(userId, identifier));
            }

            [Test]
            public void ShouldRemoveItemFromBasketAndReturnBasket()
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
                    .RemoveItemFromBasket(userId, identifier)
                    .ShouldBe(basket);

                _basketRepository
                    .Received()
                    .RemoveItemFromUserBasket(userId, productId);
            }
        }
    }
}
