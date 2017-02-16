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
    public class BasketManagerTests 
    {
        private BasketManager _basketManager;

        [SetUp]
        public void Setup()
        {
            _basketManager = new BasketManager();
        }

        [Test]
        public void ShouldReturnEmptyBasketForNewUser()
        {
            var basket = _basketManager.GetBasket("User");

            basket.ShouldNotBeNull();
            basket.Count.ShouldBe(0);
        }

        [Test]
        public void ShouldAddItemToBasketOnIncrement()
        {
            var user = "user";
            var productId = 1;

            _basketManager.AddItemToUserBasket(user, productId);

            var basket = _basketManager.GetBasket(user);

            basket.ShouldNotBeNull();
            basket.Count.ShouldBe(1);
            basket.Single().ProductId.ShouldBe(productId);
            basket.Single().ItemCount.ShouldBe(1);
        }

        [Test]
        public void ShouldAddMultipleItemsToBasketOnIncrement()
        {
            var user = "user";
            var productId = 1;
            var count = 5;

            for (int i = 0; i < count; i++)
            {
                _basketManager.AddItemToUserBasket(user, productId);
            }

            var basket = _basketManager.GetBasket(user);

            basket.ShouldNotBeNull();
            basket.Count.ShouldBe(1);
            basket.Single().ProductId.ShouldBe(productId);
            basket.Single().ItemCount.ShouldBe(count);
        }

        [Test]
        public void ShouldNotRemoveItemsBeyondZero()
        {
            var user = "user";
            var productId = 1;

            _basketManager.RemoveItemFromUserBasket(user, productId);

            var basket = _basketManager.GetBasket(user);
            basket.ShouldBeEmpty();
        }

        [Test]
        public void ShouldAndANdRemoveFromBasketAndITemShouldBeRemoved()
        {
            var user = "user";
            var productId = 1;

            _basketManager.AddItemToUserBasket(user, productId);
            _basketManager.RemoveItemFromUserBasket(user, productId);

            var basket = _basketManager.GetBasket(user);
            basket.ShouldBeEmpty();
        }
    }
}
