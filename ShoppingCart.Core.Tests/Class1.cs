using NUnit.Framework;
using ShoppingCart.Core.Controllers;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Core.Tests
{
    
    public class Tests
    {
        [Test]
        public void Test()
        {
            var controller = new ValuesController();

            false.ShouldBe(true);
        }
    }
}
