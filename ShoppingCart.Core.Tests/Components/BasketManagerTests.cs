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
        private BasketRepository _basketRepository;

        [SetUp]
        public void Setup()
        {
            _basketRepository = new BasketRepository();
        }

        [Test]
        public void ShouldReturnEmptyBasketForNewUser()
        {
            var basket = _basketRepository.GetBasket("User");

            basket.ShouldNotBeNull();
            basket.Count.ShouldBe(0);
        }

        [Test]
        public void ShouldAddItemToBasketOnIncrement()
        {
            var user = "user";
            var productId = 1;

            _basketRepository.AddItemToUserBasket(user, productId);

            var basket = _basketRepository.GetBasket(user);

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
                _basketRepository.AddItemToUserBasket(user, productId);
            }

            var basket = _basketRepository.GetBasket(user);

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

            _basketRepository
                .RemoveItemFromUserBasket(user, productId)
                .ShouldBeFalse();

            var basket = _basketRepository.GetBasket(user);
            basket.ShouldBeEmpty();
        }

        [Test]
        public void ShouldAndANdRemoveFromBasketAndITemShouldBeRemoved()
        {
            var user = "user";
            var productId = 1;

            _basketRepository.AddItemToUserBasket(user, productId);
            _basketRepository
                .RemoveItemFromUserBasket(user, productId)
                .ShouldBeTrue();

            var basket = _basketRepository.GetBasket(user);
            basket.ShouldBeEmpty();
        }

        [Test]
        public void ShouldReturnEmptyZeroItemWhenNothingFoundInBasket()
        {
            var user = "user";
            var productId = 1;

            var item = _basketRepository
                .GetBasketItem(user, productId);

            item.ProductId.ShouldBe(productId);
            item.ItemCount.ShouldBe(0);
        }
    }
}
