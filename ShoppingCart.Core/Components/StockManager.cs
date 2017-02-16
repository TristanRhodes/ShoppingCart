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
    }
}
