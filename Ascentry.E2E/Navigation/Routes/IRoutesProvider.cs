using Ascentry.E2E.Enums;

namespace Ascentry.E2E.Navigations.Routes
{
    internal interface IRoutesProvider
    {
        string Get(RouteKeyEnum route);
    }
}
