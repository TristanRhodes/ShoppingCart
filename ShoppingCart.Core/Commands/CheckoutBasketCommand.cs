using ShoppingCart.Core.Components;
using ShoppingCart.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCart.Core.Commands
{
    public interface ICheckoutBasketCommand
    {
        AvailabilityCheckResults CanCheckoutBasketCheck(
           string userId);

        Invoice CheckoutBasket(
            string userId);
    }

    public class CheckoutBasketCommand : ICheckoutBasketCommand
    {
        private IStockRepository _stockRepository;
        private IBasketRepository _basketRepository;

        public CheckoutBasketCommand(
            IStockRepository stockRepository,
            IBasketRepository basketRepository)
        {
            _stockRepository = stockRepository;
            _basketRepository = basketRepository;
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
