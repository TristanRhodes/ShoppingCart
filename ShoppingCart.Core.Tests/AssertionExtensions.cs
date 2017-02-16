using ShoppingCart.Core.Components;
using ShoppingCart.Core.Model;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Core.Tests
{
    public static class AssertionExtensions
    {
        public static void ShouldBeFullyPopulated(this DataItem item)
        {
            item.Id.ShouldBeGreaterThan(0);
            item.Name.ShouldNotBeNull();
            item.Description.ShouldNotBeNull();
            item.Stock.ShouldBeGreaterThan(0);
            item.Price.ShouldBeGreaterThan(0);
        }
    }
}
