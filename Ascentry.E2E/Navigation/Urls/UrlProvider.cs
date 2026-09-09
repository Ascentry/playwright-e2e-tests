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

        static UrlProvider()
        {
            var product = GetProduct();
            _routesProvider = RouteProviderFactory.Create(product);
        }

        public static string Get(RouteKeyEnum key)
        {
            var route = _routesProvider.Get(key);
            string rootUrlEnvVar = PlaywrightSettings.GetDescription(nameof(PlaywrightSettings.RootUrl));
            var rootUrl = Environment.GetEnvironmentVariable(rootUrlEnvVar);

            if (string.IsNullOrEmpty(rootUrl)) {
                rootUrl = TestConfiguration.Settings.RootUrl;
            }

            return $"{rootUrl}{route}";
        }

        private static ProductEnum GetProduct()
        {
            string productEnvVar = PlaywrightSettings.GetDescription(nameof(PlaywrightSettings.Product));
            var product = Environment.GetEnvironmentVariable(productEnvVar);

            if (string.IsNullOrEmpty(product))
            {
                product = TestConfiguration.Settings.Product;
            }

            if (Enum.TryParse<ProductEnum>(product, true, out var value))
            {
                return value;
            }

            throw new InvalidOperationException($"'{product}' is not a valid {nameof(ProductEnum)}.");

        }
    }
}