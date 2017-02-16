using ShoppingCart.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Core.Components
{
    public interface IStockManager
    {
        List<StockItem> GetStock();

        StockItem GetStockItem(int stockItemId);

        StockItem GetStockItem(string stockName);

        void AddStock(int productId);

        void RemoveStock(int productId);
    }
}
