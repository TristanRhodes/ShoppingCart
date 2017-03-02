using ShoppingCart.Core.Model;
using System.Collections.Generic;

namespace ShoppingCart.Core.Components
{
    public interface IBasketManager
    {
        AvailabilityCheckResults CanAddItemsToBasketCheck(string userId, List<BasketItem> products);

        List<BasketItem> AddItemsToBasket(string userId, List<BasketItem> products);
        
        bool CanAddItemToBasket(string userId, int productId);

        List<BasketItem> AddItemToBasket(string userId, int id);

        AvailabilityCheckResults CanCheckoutBasketCheck(string userId);

        List<BasketItem> RemoveItemFromBasket(string userId, int productId);
        
        Invoice CheckoutBasket(string userId);
    }
}
