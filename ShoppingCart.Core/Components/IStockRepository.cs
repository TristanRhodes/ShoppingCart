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

    public static class IStockRepositoryExtensions
    {
        public static StockItem GetStockItem(this IStockRepository repository, int? productId, string productName)
        {
            if (productId.HasValue)
                return repository.GetStockItem(productId.Value);
            else
                return repository.GetStockItem(productName);
        }

        public static List<StockItem> GetProducts(this IStockRepository repository, params int[] productIds)
        {
            return productIds
                .Select(id => repository.GetStockItem(id))
                .ToList();
        }
    }

}
