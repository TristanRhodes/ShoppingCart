using ShoppingCart.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Core.Components
{
    /// <summary>
    /// In memory stock management class. Should be loaded as singleton.
    /// </summary>
    public class StockManager : IStockManager
    {
        private List<StockItem> _stock;

        public StockManager(IDataImporter importer)
        {
            _stock = importer
                .ImportStock();
        }

        public List<StockItem> GetStock()
        {
            return _stock;
        }

        public StockItem GetStockItem(int stockItemId)
        {
            return _stock.SingleOrDefault(s => s.Id == stockItemId);
        }

        public StockItem GetStockItem(string stockName)
        {
            return _stock.SingleOrDefault(s => s.Name.ToLower() == stockName.ToLower());
        }

        public void AddStock(int productId)
        {
            var item = GetStockItem(productId);
            if (item == null)
                throw new KeyNotFoundException("Product not found: " + productId);

            item.Stock++;
        }

        public bool RemoveStock(int productId, int quantity = 1)
        {
            var item = GetStockItem(productId);
            if (item == null)
                return false;

            if (item.Stock == 0)
                return false;

            item.Stock--;
            return true;
        }
    }
}
