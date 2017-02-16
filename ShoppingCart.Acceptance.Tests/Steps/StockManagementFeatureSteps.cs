using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace ShoppingCart.Acceptance.Tests.Steps
{
    [Binding]
    public class StockManagementFeatureSteps
    {
        private readonly Uri AppUrl = new Uri("http://localhost:51998/");
        private HttpClient _client;

        public List<dynamic> StockData
        {
            get { return (List<dynamic>)ScenarioContext.Current["StockData"]; }
            set { ScenarioContext.Current["StockData"] = value; }
        }

        public StockManagementFeatureSteps()
        {
            _client = new HttpClient();
            _client.BaseAddress = AppUrl;
        }

        [Given(@"The service is running")]
        public void GivenTheServiceIsRunning()
        {
            var response = _client
                .GetAsync("heartbeat")
                .Result;

            response.IsSuccessStatusCode.ShouldBe(true);
        }

        [When(@"I request a list of stock items")]
        public void WhenIRequestAListOfStockItems()
        {
            var response = _client
                .GetAsync("api/stock")
                .Result;

            StockData = response
                .Content
                .ReadAsAsync<List<dynamic>>()
                .Result;
        }

        [Then(@"I recieve items with name, description, identifier and quantity.")]
        public void ThenRecieveItemsWithNameDescriptionIdentifierAndQuantity()
        {
            StockData.Count.ShouldBeGreaterThan(0);
            AssertionExtensions.ShouldBeFullyPopulatedStockData(StockData.First());
        }
    }
}
