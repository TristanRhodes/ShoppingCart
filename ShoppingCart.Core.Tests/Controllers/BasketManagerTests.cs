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
    public abstract class BasketManagerTests
    {
        protected IBasketRepository _basketRepository;
        protected IStockRepository _stockRepository;
            
        protected BasketManager _basketManager;

        [SetUp]
        public void Setup()
        {
            _stockRepository = Substitute.For<IStockRepository>();
            _basketRepository = Substitute.For<IBasketRepository>();
            _basketManager = new BasketManager(_stockRepository, _basketRepository);
        }

        public class CanAddItemToBasketCheck : BasketManagerTests
        {
            [Test]
            public void ShouldReturnInvalidIdentifierIfTheIdentifierSuppliedIsInvalid()
            {
                var userId = "userId";
                var identifier = new ProductIdentifier(null, null);

                _basketManager
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

                _basketManager
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

                _basketManager
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

                _basketManager
                    .CanAddItemToBasketCheck(userId, identifier)
                    .ShouldBe(BasketOperationStatus.Ok);
            }
        }

        public class AddItemToBasket : BasketManagerTests
        {
            [Test]
            public void ShouldThrowInvalidIdentifierExceptionIfTheIdentifierSuppliedIsInvalid()
            {
                var userId = "userId";
                var identifier = new ProductIdentifier(null, null);

                Should.Throw<ApplicationException>(() => _basketManager
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

                Should.Throw<ApplicationException>(() => _basketManager
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

                _basketManager
                    .AddItemToBasket(userId, identifier)
                    .ShouldBe(basket);

                _basketRepository
                    .Received()
                    .AddItemToUserBasket(userId, productId);
            }
        }

        public class CanAddItemsToBasketCheck : BasketManagerTests
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

                var check = _basketManager
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

                var check = _basketManager
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

                var check = _basketManager
                    .CanAddItemsToBasketCheck(userId, basket);

                check.Available.ShouldBeTrue();
                check.ProductsNotAvailable.ShouldBeEmpty();
                check.ProductsNotFound.ShouldBeEmpty();
            }
        }

        public class AddItemsToBasket : BasketManagerTests
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

                _basketManager
                    .AddItemsToBasket(userId, basket)
                    .ShouldBe(basket);

                _basketRepository
                    .Received()
                    .AddItemToUserBasket(userId, basketItem.ProductId, basketItem.ItemCount);
            }
        }

        public class CanCheckoutBasketCheck : BasketManagerTests
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

                var check = _basketManager
                    .CanCheckoutBasketCheck(userId);

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

                var check = _basketManager
                    .CanCheckoutBasketCheck(userId);

                check.Available.ShouldBeFalse();
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

                var check = _basketManager
                    .CanCheckoutBasketCheck(userId);

                check.Available.ShouldBeTrue();
                check.ProductsNotAvailable.ShouldBeEmpty();
                check.ProductsNotFound.ShouldBeEmpty();
            }
        }

        public class CheckoutBasket : BasketManagerTests
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

                var invoice = _basketManager
                    .CheckoutBasket(userId);

                _stockRepository
                    .Received()
                    .RemoveStock(basketItem.ProductId, basketItem.ItemCount);

                invoice.Items.Count.ShouldBe(1);
            }
        }

        public class CanRemoveItemFromBasketCheck : BasketManagerTests
        {
            [Test]
            public void ShouldReturnInvalidIdentifierIfTheIdentifierSuppliedIsInvalid()
            {
                var userId = "userId";
                var identifier = new ProductIdentifier(null, null);

                _basketManager
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

                _basketManager
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

                _basketManager
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

                _basketManager
                    .CanRemoveItemFromBasketCheck(userId, identifier)
                    .ShouldBe(BasketOperationStatus.Ok);
            }
        }
        
        public class RemoveItemFromBasket : BasketManagerTests
        {
            [Test]
            public void ShouldThrowInvalidIdentifierExceptionIfTheIdentifierSuppliedIsInvalid()
            {
                var userId = "userId";
                var identifier = new ProductIdentifier(null, null);

                Should.Throw<ApplicationException>(() => _basketManager
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

                Should.Throw<ApplicationException>(() => _basketManager
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

                _basketManager
                    .RemoveItemFromBasket(userId, identifier)
                    .ShouldBe(basket);

                _basketRepository
                    .Received()
                    .RemoveItemFromUserBasket(userId, productId);
            }
        }
    }
}
