using ShoppingCart.Core.Components;
using ShoppingCart.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCart.Core.Commands
{
    public interface IAddItemToBasketCommand
    {
        BasketOperationStatus CanAddItemToBasketCheck(
            string userId,
            ProductIdentifier identifier);

        List<BasketItem> AddItemToBasket(
            string userId,
            ProductIdentifier identifier);
    }

    public class AddItemToBasketCommand : IAddItemToBasketCommand
    {
        private IStockRepository _stockRepository;
        private IBasketRepository _basketRepository;

        public AddItemToBasketCommand(
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
    }
}
