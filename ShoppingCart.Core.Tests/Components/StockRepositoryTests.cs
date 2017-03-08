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
    public class StockRepositoryTests
    {
        private IDataImporter _importer;
        private StockRepository _stockRepository;

        [Test]
        public void ShouldGetStock()
        {
            var stock = InitializeWithStock();

            _stockRepository
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

            _stockRepository
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

            _stockRepository
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

            _stockRepository
                .AddStock(stockId);

            stockItem =_stockRepository
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

            _stockRepository
                .RemoveStock(stockId)
                .ShouldBeTrue();

            stockItem = _stockRepository
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
            _stockRepository
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

            _stockRepository = new StockRepository(_importer);

            return items;
        }
    }
}
