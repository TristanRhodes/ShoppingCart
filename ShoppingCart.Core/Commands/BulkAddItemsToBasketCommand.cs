using ShoppingCart.Core.Components;
using ShoppingCart.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCart.Core.Commands
{
    public interface IBulkAddItemsToBasketCommand
    {
        AvailabilityCheckResults CanAddItemsToBasketCheck(
          string userId,
          List<BasketItem> products);

        List<BasketItem> AddItemsToBasket(
            string userId,
            List<BasketItem> products);
    }

    public class BulkAddItemsToBasketCommand : IBulkAddItemsToBasketCommand
    {
        private IStockRepository _stockRepository;
        private IBasketRepository _basketRepository;

        public BulkAddItemsToBasketCommand(
            IStockRepository stockRepository,
            IBasketRepository basketRepository)
        {
            _stockRepository = stockRepository;
            _basketRepository = basketRepository;
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
                    results.Result = AvailabilityCheckStatus.ProductsNotFound;
                    results.ProductsNotFound.Add(addToBasketItem.ProductId);
                    continue;
                }

                var basketItem = _basketRepository.GetBasketItem(userId, addToBasketItem.ProductId);
                if (!product.HasSufficientStockFor(basketItem.ItemCount + addToBasketItem.ItemCount))
                {
                    results.Result = AvailabilityCheckStatus.InsufficientStock;
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
    }
}
