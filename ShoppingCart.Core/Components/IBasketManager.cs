using ShoppingCart.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Core.Components
{
    public interface IBasketManager
    {
        List<BasketItem> GetBasket(string userId);

        BasketItem GetBasketItem(string userId, int id);

        void AddItemToUserBasket(string userId, int productId, int quantity = 1);

        bool RemoveItemFromUserBasket(string userId, int productId);
    }
}
