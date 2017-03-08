using ShoppingCart.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCart.Core.Components
{
    public class BasketManager : IBasketManager
    {
        private IStockRepository _stockRepository;
        private IBasketRepository _basketRepository;

        public BasketManager(
            IStockRepository stockRepository,
            IBasketRepository basketRepository)
        {
            _stockRepository = stockRepository;
            _basketRepository = basketRepository;
        }

        public BasketOperationStatus CanAddItemToBasketCheck(
            string userId,
            ProductIdentifier identifier)
        {
            if (identifier.HasInvalidProductIdentifiers())
                return BasketOperationStatus.InvalidIdentifier;

            var stockItem = _stockRepository.GetStockItem(identifier);
            if (stockItem == null)
                return BasketOperationStatus.ProductNotFound;

            var basketItem = _basketRepository.GetBasketItem(userId, stockItem.Id);

            return stockItem.HasSufficientStockFor(basketItem.ItemCount + 1) ?
                BasketOperationStatus.Ok :
                BasketOperationStatus.InsufficientStock;
        }

        public List<BasketItem> AddItemToBasket(
            string userId,
            ProductIdentifier identifier)
        {
            if (identifier.HasInvalidProductIdentifiers())
                throw new ApplicationException("Invalid product identifier");

            var stockItem = _stockRepository.GetStockItem(identifier);
            if (stockItem == null)
                throw new ApplicationException("Product Not Found: " + identifier);

            _basketRepository.AddItemToUserBasket(userId, stockItem.Id);
            return _basketRepository.GetBasket(userId);
        }

        public AvailabilityCheckResults CanAddItemsToBasketCheck(
            string userId,
            List<BasketItem> products)
        {
            var results = new AvailabilityCheckResults();

            foreach (var addToBasketItem in products)
            {
                var product = _stockRepository.GetStockItem(addToBasketItem.ProductId);
                if (product == null)
                {
                    results.Available = false;
                    results.ProductsNotFound.Add(addToBasketItem.ProductId);
                    continue;
                }

                var basketItem = _basketRepository.GetBasketItem(userId, addToBasketItem.ProductId);
                if (!product.HasSufficientStockFor(basketItem.ItemCount + addToBasketItem.ItemCount))
                {
                    results.Available = false;
                    results.ProductsNotAvailable.Add(product.Name);
                    continue;
                }
            }

            return results;
        }

        public List<BasketItem> AddItemsToBasket(
            string userId,
            List<BasketItem> products)
        {
            foreach (var item in products)
            {
                _basketRepository.AddItemToUserBasket(userId, item.ProductId, item.ItemCount);
            }

            return _basketRepository.GetBasket(userId);
        }

        public AvailabilityCheckResults CanCheckoutBasketCheck(
            string userId)
        {
            var results = new AvailabilityCheckResults();

            var basket = _basketRepository.GetBasket(userId);

            foreach (var basketItem in basket)
            {
                var product = _stockRepository.GetStockItem(basketItem.ProductId);
                if (product == null)
                {
                    results.Available = false;
                    results.ProductsNotFound.Add(basketItem.ProductId);
                    continue;
                }

                if (!product.HasSufficientStockFor(basketItem.ItemCount))
                {
                    results.Available = false;
                    results.ProductsNotAvailable.Add(product.Name);
                    continue;
                }
            }

            return results;
        }

        public Invoice CheckoutBasket(
            string userId)
        {
            var basket = _basketRepository.GetBasket(userId);
            var products = GetProducts(basket);

            foreach (var basketItem in basket)
            {
                _stockRepository.RemoveStock(basketItem.ProductId, basketItem.ItemCount);
            }

            return GenerateInvoice(userId, basket, products);
        }

        public BasketOperationStatus CanRemoveItemFromBasketCheck(
            string userId,
            int productId)
        {
            var stockItem = _stockRepository.GetStockItem(productId);
            if (stockItem == null)
                return BasketOperationStatus.ProductNotFound;

            var basketItem = _basketRepository.GetBasketItem(userId, productId);
            if (basketItem.ItemCount == 0)
                return BasketOperationStatus.NotInBasket;

            return BasketOperationStatus.Ok;
        }

        public List<BasketItem> RemoveItemFromBasket(
            string userId, 
            int productId)
        {
            _basketRepository.RemoveItemFromUserBasket(userId, productId);
            return _basketRepository.GetBasket(userId);
        }

        private Invoice GenerateInvoice(
            string userId, 
            List<BasketItem> basket, 
            List<StockItem> products)
        {
            var invoiceItems = GenerateInvoiceItems(basket, products);

            var invoice = new Invoice();
            invoice.User = userId;
            invoice.Items = invoiceItems;
            invoice.Total = invoice
                        .Items
                        .Sum(i => i.Cost);
            return invoice;
        }

        private List<InvoiceItem> GenerateInvoiceItems(
            List<BasketItem> basket, 
            List<StockItem> products)
        {
            return basket
                .Select(b => new
                {
                    BasketItem = b,
                    StockItem = products.Single(p => p.Id == b.ProductId)
                })
                .Select(i => new InvoiceItem()
                {
                    ProductName = i.StockItem.Name,
                    Quantity = i.BasketItem.ItemCount,
                    Cost = i.BasketItem.ItemCount * i.StockItem.Price
                })
                .ToList();
        }

        private List<StockItem> GetProducts(
            IEnumerable<BasketItem> basket)
        {
            var ids = basket
                .Select(b => b.ProductId)
                .ToArray();

            return _stockRepository.GetProducts(ids);
        }
    }
}
