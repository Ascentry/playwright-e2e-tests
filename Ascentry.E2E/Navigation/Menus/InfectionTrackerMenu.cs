using Ascentry.E2E.Contracts.Common;
using Ascentry.E2E.Contracts.Enums;
using System.Collections.Generic;

namespace Ascentry.E2E.Navigations.Menus
{
    public static class InfectionTrackerMenu
    {
        public static class Epidemiology
        {
            public static class Publications
            {
                private const TranslationEnum root = TranslationEnum.Epidemiology;
                public static readonly List<MenuNode> GeneratePublications = [
                    new MenuNode(MenuLevelEnum.First, root),
                    new MenuNode(MenuLevelEnum.Second, TranslationEnum.GenerateAPost)
                ];
                public static readonly List<MenuNode> PublicationsTracking = [
                    new MenuNode(MenuLevelEnum.First, root),
                    new MenuNode(MenuLevelEnum.Second, TranslationEnum.PublicationTracking)
                ];
            }
        }

        public static class Settings
        {
            public static class Expertise
            {
                private const TranslationEnum root = TranslationEnum.Configuration_2;
                public static readonly List<MenuNode> ExpertRules = [
                   new MenuNode(MenuLevelEnum.First, root),
                   new MenuNode(MenuLevelEnum.Second, TranslationEnum.ExpertiseRules)
                ];
            }
        }

    }
}
