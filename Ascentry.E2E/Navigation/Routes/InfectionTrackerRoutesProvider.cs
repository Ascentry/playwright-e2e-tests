using Ascentry.E2E.Enums;
using System.Collections.Generic;

namespace Ascentry.E2E.Navigations.Routes
{
    internal class InfectionTrackerRoutesProvider : IRoutesProvider
    {
        private readonly Dictionary<RouteKeyEnum, string> _routes = new Dictionary<RouteKeyEnum, string>()
            {
                { RouteKeyEnum.Login, "/sign-in"},
                { RouteKeyEnum.Home, "/home"},
                { RouteKeyEnum.GeneratePublication, "/publication/generate/manually"},
                { RouteKeyEnum.ExpertRuleList, "/parameters/expert-rule"},
                { RouteKeyEnum.IaDetectionsList, "/ai-expertise/organism-clusters"}
            };

        public string Get(RouteKeyEnum key)
        {
            return _routes[key];
        }
    }
}