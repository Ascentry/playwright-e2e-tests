using Ascentry.E2E.Contracts.Enums;
using System;

namespace Ascentry.E2E.Navigations.Routes
{
    internal static class RouteProviderFactory
    {
        public static IRoutesProvider Create(ProductEnum product)
        {
            switch (product)
            {
                case ProductEnum.InfectionTracker:
                    return new InfectionTrackerRoutesProvider();
                default:
                    throw new NotSupportedException($"The value '{product}'is not supported for {nameof(ProductEnum)}");
            }
        }
    }
}
