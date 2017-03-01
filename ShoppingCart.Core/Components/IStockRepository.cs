using ShoppingCart.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Core.Components
{
    public interface IStockRepository
    {
        List<StockItem> GetStock();

        StockItem GetStockItem(int stockItemId);

        StockItem GetStockItem(string stockName);

        void AddStock(int productId);

        bool RemoveStock(int productId, int quantity = 1);
    }
}
