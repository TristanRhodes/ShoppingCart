using NSubstitute;
using NUnit.Framework;
using ShoppingCart.Core.Commands;
using ShoppingCart.Core.Components;
using ShoppingCart.Core.Model;
using Shouldly;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCart.Core.Tests.Commands
{
    public abstract class CheckoutBasketCommandTests
    {
        protected IBasketRepository _basketRepository;
        protected IStockRepository _stockRepository;

        protected CheckoutBasketCommand _command;

        [SetUp]
        public void Setup()
        {
            _stockRepository = Substitute.For<IStockRepository>();
            _basketRepository = Substitute.For<IBasketRepository>();
            _command = new CheckoutBasketCommand(_stockRepository, _basketRepository);
        }

        public class CanCheckoutBasketCheck : CheckoutBasketCommandTests
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

                _basketRepository
                    .GetBasket(userId)
                    .Returns(basket);

                _stockRepository
                    .GetStockItem(productId)
                    .Returns((StockItem)null);

                var check = _command
                    .CanCheckoutBasketCheck(userId);

                check.Result.ShouldBe(AvailabilityCheckStatus.ProductsNotFound);
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
                    Stock = 0
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

                _basketRepository
                    .GetBasket(userId)
                    .Returns(basket);

                _stockRepository
                    .GetStockItem(productId)
                    .Returns(stockItem);

                _basketRepository
                    .GetBasketItem(userId, productId)
                    .Returns(basketItem);

                var check = _command
                    .CanCheckoutBasketCheck(userId);

                check.Result.ShouldBe(AvailabilityCheckStatus.InsufficientStock);
                check.ProductsNotAvailable.Count.ShouldBe(1);
                check.ProductsNotAvailable.Single().ShouldBe(productName);
                check.ProductsNotFound.ShouldBeEmpty();
            }

            [Test]
            public void ShouldReturnAvailableWhenProductIsInStock()
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

                _basketRepository
                    .GetBasket(userId)
                    .Returns(basket);

                _stockRepository
                    .GetStockItem(productId)
                    .Returns(stockItem);

                _basketRepository
                    .GetBasketItem(userId, productId)
                    .Returns(basketItem);

                var check = _command
                    .CanCheckoutBasketCheck(userId);

                check.Result.ShouldBe(AvailabilityCheckStatus.Ok);
                check.ProductsNotAvailable.ShouldBeEmpty();
                check.ProductsNotFound.ShouldBeEmpty();
            }
        }

        public class CheckoutBasket : CheckoutBasketCommandTests
        {
            [Test]
            public void ShouldRemoveStockAndGenerateInvoice()
            {
                var userId = "userId";
                var productId = 1;

                var basketItem = new BasketItem()
                {
                    ProductId = productId,
                    ItemCount = 1
                };
                var basket = new List<BasketItem>()
                {
                    basketItem
                };

                var stockItem = new StockItem()
                {
                    Id = productId,
                    Stock = 2
                };

                _basketRepository
                    .GetBasket(userId)
                    .Returns(basket);

                _stockRepository
                    .GetStockItem(productId)
                    .Returns(stockItem);

                var invoice = _command
                    .CheckoutBasket(userId);

                _stockRepository
                    .Received()
                    .RemoveStock(basketItem.ProductId, basketItem.ItemCount);

                invoice.Items.Count.ShouldBe(1);
            }
        }
    }
}
