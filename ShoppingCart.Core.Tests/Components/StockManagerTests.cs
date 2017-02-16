using NSubstitute;
using NUnit.Framework;
using ShoppingCart.Core.Components;
using ShoppingCart.Core.Model;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Core.Tests.Components
{
    public class StockManagerTests
    {
        private IDataImporter _importer;
        private StockManager _stockManager;

        [Test]
        public void ShouldGetStock()
        {
            var stock = InitializeWithStock();

            _stockManager
                .GetStock()
                .ShouldBe(stock);
        }

        [Test]
        public void ShouldReturnItemWithMatchingIdWhenFound()
        {
            var stockId = 1;
            var stockItem = new StockItem()
            {
                Id = stockId
            };

            var stock = InitializeWithStock(stockItem);

            _stockManager
                .GetStockItem(stockId)
                .ShouldBe(stockItem);
        }

        [Test]
        public void ShouldReturnItemMatchingNameWhenFound()
        {
            var stockName = "name";
            var stockItem = new StockItem()
            {
                Name = stockName
            };

            var stock = InitializeWithStock(stockItem);

            _stockManager
                .GetStockItem(stockName)
                .ShouldBe(stockItem);
        }

        [Test]
        public void ShouldIncreaseStockCountOnIncrement()
        {
            var stockId = 1;
            var stockLevel = 0;

            var stockItem = new StockItem()
            {
                Id = stockId,
                Stock = stockLevel
            };

            var stock = InitializeWithStock(stockItem);

            _stockManager
                .AddStock(stockId);

            stockItem =_stockManager
                .GetStockItem(stockId);

            stockItem.Stock.ShouldBe(stockLevel + 1);
        }

        [Test]
        public void ShouldDecreaseStockCountOnReduceAndReturnTrue()
        {
            var stockId = 1;
            var stockLevel = 1;

            var stockItem = new StockItem()
            {
                Id = stockId,
                Stock = stockLevel
            };

            var stock = InitializeWithStock(stockItem);

            _stockManager
                .RemoveStock(stockId)
                .ShouldBeTrue();

            stockItem = _stockManager
                .GetStockItem(stockId);

            stockItem.Stock.ShouldBe(stockLevel - 1);
        }

        [Test]
        public void ShouldReturnFalseWhenReducingStockBelowZero()
        {
            var stockId = 1;
            var stockLevel = 0;

            var stockItem = new StockItem()
            {
                Id = stockId,
                Stock = stockLevel
            };

            var stock = InitializeWithStock(stockItem);
            _stockManager
                .RemoveStock(stockId)
                .ShouldBeFalse();
        }

        private List<StockItem> InitializeWithStock(params StockItem[] stock)
        {
            var items = new List<StockItem>();
            items.AddRange(stock);

            _importer = Substitute.For<IDataImporter>();
            _importer
                .ImportStock()
                .Returns(items);

            _stockManager = new StockManager(_importer);

            return items;
        }
    }
}
