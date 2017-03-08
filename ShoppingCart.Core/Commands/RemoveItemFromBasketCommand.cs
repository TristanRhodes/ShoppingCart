using ShoppingCart.Core.Components;
using ShoppingCart.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCart.Core.Commands
{
    public interface IRemoveItemFromBasketCommand
    {
        BasketOperationStatus CanRemoveItemFromBasketCheck(
            string userId,
            ProductIdentifier identifier);

        List<BasketItem> RemoveItemFromBasket(
            string userId,
            ProductIdentifier identifier);
    }

    public class RemoveItemFromBasketCommand : IRemoveItemFromBasketCommand
    {
        private IStockRepository _stockRepository;
        private IBasketRepository _basketRepository;

        public RemoveItemFromBasketCommand(
            IStockRepository stockRepository,
            IBasketRepository basketRepository)
        {
            _stockRepository = stockRepository;
            _basketRepository = basketRepository;
        }

        public BasketOperationStatus CanRemoveItemFromBasketCheck(
            string userId,
            ProductIdentifier identifier)
        {
            if (!identifier.Valid)
                return BasketOperationStatus.InvalidIdentifier;

            var stockItem = _stockRepository.GetStockItem(identifier);
            if (stockItem == null)
                return BasketOperationStatus.ProductNotFound;

            var basketItem = _basketRepository.GetBasketItem(userId, stockItem.Id);
            if (basketItem.ItemCount == 0)
                return BasketOperationStatus.NotInBasket;

            return BasketOperationStatus.Ok;
        }

        public List<BasketItem> RemoveItemFromBasket(
            string userId,
            ProductIdentifier identifier)
        {
            if (!identifier.Valid)
                throw new ApplicationException("Invalid product identifier");

            var stockItem = _stockRepository.GetStockItem(identifier);
            if (stockItem == null)
                throw new ApplicationException("Product Not Found: " + identifier);

            _basketRepository.RemoveItemFromUserBasket(userId, stockItem.Id);
            return _basketRepository.GetBasket(userId);
        }
    }
}
