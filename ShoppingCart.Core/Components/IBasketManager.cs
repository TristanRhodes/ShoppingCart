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

        void IncrementItemCount(string userId, int productId);

        void DecrementItemCount(string userId, int productId);
    }

}
