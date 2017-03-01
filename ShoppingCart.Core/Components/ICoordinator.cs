﻿using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Core.Components
{
    public interface ICoordinator
    {
        List<BasketItem> AddItemsToBasket(string userId, List<BasketItem> products);

        AvailabilityCheckResults CanAddItemsToBasketCheck(string userId, List<BasketItem> products);

        List<BasketItem> AddItemToBasket(string userId, int id);

        bool CanAddItemToBasket(string userId, int productId);

        Invoice CheckoutBasket(string userId);

        StockItem GetStockItem(int? productId, string productName);

        StockAvailabilityCheckResults UserBasketStockCheck(string userId);

        List<BasketItem> RemoveItemFromBasket(string userId, int productId);
    }

    public class Coordinator : ICoordinator
    {
        private IStockManager _stockManager;
        private IBasketManager _basketManager;

        public Coordinator(
            IStockManager stockManager,
            IBasketManager basketManager)
        {
            _stockManager = stockManager;
            _basketManager = basketManager;
        }

        public List<BasketItem> AddItemsToBasket(
            string userId, 
            List<BasketItem> products)
        {
            foreach (var item in products)
            {
                _basketManager.AddItemToUserBasket(userId, item.ProductId, item.ItemCount);
            }

            return _basketManager.GetBasket(userId);
        }

        public AvailabilityCheckResults CanAddItemsToBasketCheck(
            string userId,
            List<BasketItem> products)
        {
            var results = new AvailabilityCheckResults();

            foreach (var addToBasketItem in products)
            {
                var product = _stockManager.GetStockItem(addToBasketItem.ProductId);
                if (product == null)
                {
                    results.Available = false;
                    results.ProductsNotFound.Add(addToBasketItem.ProductId);
                    continue;
                }

                var basketItem = _basketManager.GetBasketItem(userId, addToBasketItem.ProductId);
                if (!product.HasSufficientStockFor(basketItem.ItemCount + addToBasketItem.ItemCount))
                {
                    results.Available = false;
                    results.ProductsNotAvailable.Add(product.Name);
                    continue;
                }
            }

            return results;
        }

        public StockAvailabilityCheckResults UserBasketStockCheck(
            string userId)
        {

            var results = new StockAvailabilityCheckResults();

            var basket = _basketManager.GetBasket(userId);
            var products = new List<StockItem>();

            foreach (var basketItem in basket)
            {
                var product = _stockManager.GetStockItem(basketItem.ProductId);
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

                products.Add(product);
            }

            if (results.Available)
                results.Stock = products;

            return results;
        }

        public Invoice CheckoutBasket(string userId)
        {
            var basket = _basketManager.GetBasket(userId);

            foreach (var basketItem in basket)
            {
                _stockManager.RemoveStock(basketItem.ProductId, basketItem.ItemCount);
            }

            return GenerateInvoice(userId, basket);
        }

        public StockItem GetStockItem(int? productId, string productName)
        {
            if (productId.HasValue)
                return _stockManager.GetStockItem(productId.Value);
            else
                return _stockManager.GetStockItem(productName);
        }

        private Invoice GenerateInvoice(string userId, List<BasketItem> basket, List<StockItem> products = null)
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

        private List<InvoiceItem> GenerateInvoiceItems(List<BasketItem> basket, List<StockItem> products)
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

        public bool CanAddItemToBasket(string userId, int productId)
        {
            var stockItem = _stockManager.GetStockItem(productId);
            var basketItem = _basketManager.GetBasketItem(userId, productId);

            return stockItem.HasSufficientStockFor(basketItem.ItemCount + 1);
        }

        public List<BasketItem> AddItemToBasket(string userId, int productId)
        {
            _basketManager.AddItemToUserBasket(userId, productId);
            return _basketManager.GetBasket(userId);
        }

        public List<BasketItem> RemoveItemFromBasket(string userId, int productId)
        {
            _basketManager.RemoveItemFromUserBasket(userId, productId);
            return _basketManager.GetBasket(userId);
        }
    }

    public class StockAvailabilityCheckResults : AvailabilityCheckResults
    {
        public List<StockItem> Stock { get; set; }
    }

    public class AvailabilityCheckResults
    {
        public AvailabilityCheckResults()
        {
            ProductsNotFound = new List<int>();
            ProductsNotAvailable = new List<string>();
            Available = true;
        }

        public bool Available { get; set; }

        public List<int> ProductsNotFound { get; set; }

        public List<string> ProductsNotAvailable { get; set; }

        public bool HasNotFoundProducts
        {
            get { return !Available && ProductsNotFound.Count > 0; }
        }

        public bool HasUnavailableProducts
        {
            get { return !Available && ProductsNotAvailable.Count > 0; }
        }
    }
}
