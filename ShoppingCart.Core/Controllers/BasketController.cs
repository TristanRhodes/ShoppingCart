using Microsoft.AspNetCore.Mvc;
using ShoppingCart.Core.Components;
using ShoppingCart.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Core.Controllers
{
    public class BasketController : Controller
    {
        private IStockManager _stockManager;
        private IBasketManager _basketManager;

        public BasketController(
            IStockManager stockManager,
            IBasketManager basketManager)
        {
            _stockManager = stockManager;
            _basketManager = basketManager;
        }

        [HttpGet("api/{userId}/basket")]
        public IActionResult GetBasket(
            [FromRoute]string userId)
        {
            return Json(_basketManager.GetBasket(userId));
        }

        [HttpPut("api/{userId}/basket/add")]
        public IActionResult AddToBasket(
            [FromRoute]string userId,
            [FromQuery]int? productId = null,
            [FromQuery]string productName = null)
        {
            if (HasInvalidProductIdentifiers(productId, productName))
                return BadRequest("Please supply a single value for productId or productName.");

            var product = ResolveProduct(productId, productName);
            if (product == null)
                return NotFound("Product");

            var basketItem = _basketManager.GetBasketItem(userId, product.Id);
            if (!product.HasSufficientStockFor(basketItem.ItemCount + 1))
                return BadRequest("Not Enough Stock");
            
            _basketManager.AddItemToUserBasket(userId, product.Id);

            var basket = _basketManager.GetBasket(userId);
            return Json(basket);
        }

        [HttpPost("api/{userId}/basket/add")]
        public IActionResult BulkAddToBasket(
            [FromRoute]string userId,
            [FromBody]List<BasketItem> products)
        {
            foreach(var addToBasketItem in products)
            {
                var product = _stockManager.GetStockItem(addToBasketItem.ProductId);
                var basketItem = _basketManager.GetBasketItem(userId, addToBasketItem.ProductId);

                if (!product.HasSufficientStockFor(basketItem.ItemCount + addToBasketItem.ItemCount))
                    return BadRequest("Not Enough Stock for item: " + product.Name);
            }

            foreach(var item in products)
            {
                _basketManager.AddItemToUserBasket(userId, item.ProductId, item.ItemCount);
            }

            var basket = _basketManager.GetBasket(userId);
            return Json(basket);
        }
        
        [HttpPut("api/{userId}/basket/remove")]
        public IActionResult RemoveFromBasket(
            [FromRoute]string userId,
            [FromQuery]int? productId = null,
            [FromQuery]string productName = null)
        {
            if (HasInvalidProductIdentifiers(productId, productName))
                return BadRequest("Please supply a single value for productId or productName.");

            var product = ResolveProduct(productId, productName);
            if (product == null)
                return NotFound("Product");

            _basketManager.RemoveItemFromUserBasket(userId, product.Id);

            var basket = _basketManager.GetBasket(userId);
            return Json(basket);
        }


        [HttpPut("api/{userId}/basket/checkout")]
        public IActionResult CheckoutBasket(
            [FromRoute]string userId)
        {
            var basket = _basketManager.GetBasket(userId);
            var products = new List<StockItem>();

            // Validate Capacity
            foreach (var basketItem in basket)
            {
                var product = _stockManager.GetStockItem(basketItem.ProductId);
                if (product == null)
                    return BadRequest("Product not found: " + basketItem.ProductId);

                if (!product.HasSufficientStockFor(basketItem.ItemCount))
                    return BadRequest("Not Enough Stock for item: " + product.Name);

                products.Add(product);
            }


            // Apply
            foreach (var basketItem in basket)
            {
                _stockManager.RemoveStock(basketItem.ProductId, basketItem.ItemCount);
            }

            var invoiceItems = GenerateInvoiceItems(basket, products);
            var invoice = GenerateInvoice(userId, basket, products);
            return Json(invoice);
        }

        private static Invoice GenerateInvoice(string userId, List<BasketItem> basket, List<StockItem> products)
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

        private static List<InvoiceItem> GenerateInvoiceItems(List<BasketItem> basket, List<StockItem> products)
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

        private StockItem ResolveProduct(int? productId, string productName)
        {
            if (productId.HasValue)
                return _stockManager.GetStockItem(productId.Value);
            else
                return _stockManager.GetStockItem(productName);
        }

        private static bool HasInvalidProductIdentifiers(int? productId, string productName)
        {
            return (!productId.HasValue && string.IsNullOrEmpty(productName))
                || (productId.HasValue && !string.IsNullOrEmpty(productName));
        }
    }
}
