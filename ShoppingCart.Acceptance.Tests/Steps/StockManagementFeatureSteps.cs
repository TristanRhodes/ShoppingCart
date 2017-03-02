using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

        public string UserName
        {
            get
            {
                if (!ScenarioContext.Current.ContainsKey("UserName"))
                    ScenarioContext.Current["UserName"] = Guid.NewGuid().ToString();

                return (string)ScenarioContext.Current["UserName"];
            }
        }

        public List<dynamic> StockData
        {
            get { return (List<dynamic>)ScenarioContext.Current["StockData"]; }
            set { ScenarioContext.Current["StockData"] = value; }
        }

        public List<dynamic> Basket
        {
            get { return (dynamic)ScenarioContext.Current["Basket"]; }
            set { ScenarioContext.Current["Basket"] = value; }
        }

        public dynamic StockItem
        {
            get { return (dynamic)ScenarioContext.Current["StockItem"]; }
            set { ScenarioContext.Current["StockItem"] = value; }
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

            response
                .StatusCode
                .ShouldBe(HttpStatusCode.OK, "Heartbeat check failed");
        }

        [When(@"I request a list of stock items")]
        public void WhenIRequestAListOfStockItems()
        {
            var response = _client
                .GetAsync("api/stock")
                .Result;

            response
                .StatusCode
                .ShouldBe(HttpStatusCode.OK, "Get Stock Failed");

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

        [When(@"I add a stocked item to a users basket")]
        public void WhenIAddAStockedItemToAUsersBasket()
        {
            StockItem = StockData.First(s => s.stock > 0);
            var url = string.Format("api/{0}/basket/add?productId={1}", UserName, (int)StockItem.id);

            var response = _client
                .PutAsync(url, null)
                .Result;

            response
                .StatusCode
                .ShouldBe(HttpStatusCode.OK, "Add to basket failed");

            Basket = response
                .Content
                .ReadAsAsync<List<dynamic>>()
                .Result;
        }

        [Then(@"The basket I recieve contains this item")]
        public void ThenTheBasketIRecieveContainsThisItem()
        {
            Basket.Count.ShouldBeGreaterThan(0);

            var basketItem = Basket.First();
            ((int)basketItem.productId).ShouldBe((int)StockItem.id);
            ((int)basketItem.itemCount).ShouldBe(1);
        }

        [When(@"I request a users basket")]
        public void WhenIRequestAUsersBasket()
        {
            var url = string.Format("api/{0}/basket", UserName);

            var response = _client
                .GetAsync(url)
                .Result;

            response
                .StatusCode
                .ShouldBe(HttpStatusCode.OK, "Get basket failed");

            Basket = response
                .Content
                .ReadAsAsync<List<dynamic>>()
                .Result;
        }
    }
}
