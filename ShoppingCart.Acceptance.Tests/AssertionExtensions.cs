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
        public static void ShouldBeFullyPopulatedStockData(dynamic stockData)
        {
            ((int?)stockData.id).ShouldNotBeNull();
            ((string)stockData.name).ShouldNotBeNullOrEmpty();
            ((string)stockData.description).ShouldNotBeNullOrEmpty();
            ((int?)stockData.stock).ShouldNotBeNull();
            ((decimal?)stockData.price).ShouldNotBeNull();
        }
    }
}
