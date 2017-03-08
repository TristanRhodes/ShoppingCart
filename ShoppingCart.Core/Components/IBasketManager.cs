using ShoppingCart.Core.Model;
using System.Collections.Generic;

namespace ShoppingCart.Core.Components
{
    public interface IBasketManager
    {
        BasketOperationStatus CanAddItemToBasketCheck(string userId, ProductIdentifier identifier);

        List<BasketItem> AddItemToBasket(string userId, ProductIdentifier identifier);

        AvailabilityCheckResults CanAddItemsToBasketCheck(string userId, List<BasketItem> products);

        List<BasketItem> AddItemsToBasket(string userId, List<BasketItem> products);

        BasketOperationStatus CanRemoveItemFromBasketCheck(string userId, ProductIdentifier identifier);

        List<BasketItem> RemoveItemFromBasket(string userId, ProductIdentifier identifier);

        AvailabilityCheckResults CanCheckoutBasketCheck(string userId);

        Invoice CheckoutBasket(string userId);
    }

    public class ProductIdentifier
    {
        public ProductIdentifier(int? productId, string productName)
        {
            ProductId = productId;
            ProductName = productName;
        }

        public ProductIdentifier(int productId)
        {
            ProductId = productId;
            ProductName = null;
        }

        public ProductIdentifier(string productName)
        {
            ProductId = null;
            ProductName = productName;
        }

        public int? ProductId { get; set; }

        public string ProductName { get; set; }

        public bool HasInvalidProductIdentifiers()
        {
            return (!ProductId.HasValue && string.IsNullOrEmpty(ProductName))
                || (ProductId.HasValue && !string.IsNullOrEmpty(ProductName));
        }
    }
}
