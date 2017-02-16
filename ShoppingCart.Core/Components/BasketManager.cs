﻿using ShoppingCart.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Core.Components
{
    public class BasketManager : IBasketManager
    {
        private Dictionary<string, List<BasketItem>> baskets 
            = new Dictionary<string, List<BasketItem>>();
        
        public List<BasketItem> GetBasket(string userId)
        {
            if (!baskets.ContainsKey(userId))
                return new List<BasketItem>();

            return baskets[userId];
        }

        public void AddItemToUserBasket(string userId, int productId)
        {
            var item = GetOrCreateItem(userId, productId);
            item.ItemCount++;
        }

        public bool RemoveItemFromUserBasket(string userId, int productId)
        {
            var item = GetItem(userId, productId);
            if (item == null)
                return false;

            if (item.ItemCount <= 0)
                return false;

            item.ItemCount--;

            var cleanup = item.ItemCount <= 0;
            if (cleanup)
                RemoveBasketItem(userId, productId);

            return true;
        }

        private BasketItem GetOrCreateItem(string userId, int productId)
        {
            var item = GetItem(userId, productId);

            if (item == null)
            {
                item = new BasketItem();
                item.ProductId = productId;
                item.ItemCount = 0;
                baskets[userId].Add(item);
            }

            return item;
        }

        private BasketItem GetItem(string userId, int productId)
        {
            if (!baskets.ContainsKey(userId))
                baskets.Add(userId, new List<BasketItem>());

            var items = baskets[userId];
            return items.SingleOrDefault(i => i.ProductId == productId);
        }

        private void RemoveBasketItem(string userId, int productId)
        {
            var items = baskets[userId];
            var item = items.SingleOrDefault(i => i.ProductId == productId);

            if (item != null)
                items.Remove(item);
        }
    }
}
