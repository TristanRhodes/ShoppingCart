using NUnit.Framework;
using ShoppingCart.Core.Components;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Core.Tests.Components
{
    public class DataImporterTests
    {
        [Test]
        public void ShouldImportData()
        {
            var dataFile = "TestData/example_data.csv";
            var dataImporter = new DataImporter(dataFile);

            var data = dataImporter.ImportStock();

            data.Count.ShouldBeGreaterThan(0);
            data.First().ShouldBeFullyPopulated();
        }
    }
}
