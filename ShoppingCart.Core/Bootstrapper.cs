using Microsoft.Extensions.DependencyInjection;
using ShoppingCart.Core.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCart.Core
{
    public static class Bootstrapper
    {
        public static IServiceCollection UseImportedStockFile(this IServiceCollection services, string file)
        {
            services.AddSingleton<IDataImporter>((s) => new DataImporter(file));
            return services;
        }

        public static IServiceCollection AddShoppingCartComponents(this IServiceCollection services)
        {
            services.AddSingleton<IBasketRepository, BasketRepository>();
            services.AddSingleton<IStockRepository, StockRepository>();
            services.AddSingleton<IBasketManager, BasketManager>();

            return services;
        }
    }
}
