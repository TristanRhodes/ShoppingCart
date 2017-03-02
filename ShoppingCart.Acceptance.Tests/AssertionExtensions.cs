using ShoppingCart.Core.Model;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Acceptance.Tests
{
    public static class AssertionExtensions
    {
        public static void ShouldBeFullyPopulatedStockData(StockItem stockItem)
        {
            stockItem.Id.ShouldNotBeNull();
            stockItem.Name.ShouldNotBeNullOrEmpty();
            stockItem.Description.ShouldNotBeNullOrEmpty();
            stockItem.Stock.ShouldNotBeNull();
            stockItem.Price.ShouldNotBeNull();
        }
    }
}
