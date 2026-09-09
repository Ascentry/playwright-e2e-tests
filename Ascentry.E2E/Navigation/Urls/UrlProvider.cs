using Ascentry.E2E.Configurations;
using Ascentry.E2E.Contracts.Enums;
using Ascentry.E2E.Enums;
using Ascentry.E2E.Navigations.Routes;
using System;

namespace Ascentry.E2E.Navigations.Urls
{
    internal static class UrlProvider
    {
        private static readonly IRoutesProvider _routesProvider;
        private const string ProductEnvironmentVariable = "PLAYWRIGHT_PRODUCT";

        static UrlProvider()
        {
            var product = GetProduct();
            _routesProvider = RouteProviderFactory.Create(product);
        }

        public static string Get(RouteKeyEnum key)
        {
            var route = _routesProvider.Get(key);
            return $"{TestConfiguration.Settings.RootUrl}{route}";
        }

        private static ProductEnum GetProduct()
        {
            var value = Environment.GetEnvironmentVariable(ProductEnvironmentVariable);

            if (string.IsNullOrEmpty(value))
            {
                value = TestConfiguration.Settings.Product;
            }

            if (Enum.TryParse<ProductEnum>(value, true, out var product))
            {
                return product;
            }

            throw new InvalidOperationException($"'{value}' is not a valid {nameof(ProductEnum)}.");

        }
    }
}