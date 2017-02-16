using NSubstitute;
using NUnit.Framework;
using ShoppingCart.Core.Model;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Core.Components
{
    public class StockManagerTests
    {
        [Test]
        public void ShouldGetStock()
        {
            var stock = new List<StockItem>();
            var importer = Substitute.For<IDataImporter>();

            importer
                .ImportStock()
                .Returns(stock);

            var stockManager = new StockManager(importer);

            stockManager
                .GetStock()
                .ShouldBe(stock);
        }
    }


}
