using Ascentry.E2E.Contracts.Common;
using Ascentry.E2E.Contracts.Enums;
using Ascentry.E2E.Contracts.Layout;
using Ascentry.E2E.Core.Translations;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ascentry.E2E.Navigations.Menus
{

    internal class NavigationMenu: INavigationMenu
    {
        private readonly IPage _page;

        internal NavigationMenu(IPage page)
        {
            _page = page;
        }

        /// <summary>
        /// Go to the specified route represented by a list of nodes
        /// </summary>
        /// <param name="nodes">List of the nodes that compose the navigation route</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public async Task GoToAsync(IEnumerable<MenuNode> nodes)
        {
            var mainMenu = _page.Locator("byg-site-menu");
            var subMenu = _page.Locator("#sub-menu");

            await Assertions.Expect(mainMenu).ToBeVisibleAsync();

            foreach (var node in nodes)
            {
                var label = TranslationProvider.Get(node.Key);

                switch (node.Level)
                {
                    case MenuLevelEnum.First:
                        await OpenMainMenuAsync(mainMenu, subMenu, label);
                        break;

                    case MenuLevelEnum.Second:
                        await ClickSubMenuLinkAsync(subMenu, label);
                        break;

                    default:
                        throw new NotSupportedException(
                            $"{node.Level} n'est pas une valeur supportée pour " +
                            $"{nameof(MenuLevelEnum)}.");
                }
            }

        }

        private async Task OpenMainMenuAsync(ILocator mainMenu, ILocator subMenu, string label)
        {
            var menuItem = mainMenu.Locator($"div.menu-item-container:has(span.menu-item-label:has-text('{label}'))");

            await Assertions.Expect(menuItem).ToHaveCountAsync(1);
            await Assertions.Expect(menuItem).ToBeVisibleAsync();

            // Bad practice but no choice because DOM is not stabalised before the click on the main manu.
            await _page.WaitForTimeoutAsync(2_000);

            await menuItem.ClickAsync();

            // Attendre l’état ouvert du sous-menu.
            await Assertions.Expect(subMenu).ToHaveClassAsync(
                new Regex(@"\bopen\b"));

            await Assertions.Expect(subMenu.Locator("byg-sub-menu"))
                .ToBeVisibleAsync();
        }

        private async Task ClickSubMenuLinkAsync(ILocator subMenu, string label)
        {
            await Assertions.Expect(subMenu).ToHaveClassAsync(
                new Regex(@"\bopen\b"));

            // Le Locator est créé après l’ouverture du sous-menu.
            var link = subMenu.GetByRole(
                AriaRole.Link,
                new LocatorGetByRoleOptions
                {
                    Name = label,
                    Exact = true
                });

            await Assertions.Expect(link).ToHaveCountAsync(1);
            await Assertions.Expect(link).ToBeVisibleAsync();
            await Assertions.Expect(link).ToBeEnabledAsync();

            var href = await link.GetAttributeAsync("href");

            if (string.IsNullOrWhiteSpace(href))
            {
                throw new InvalidOperationException(
                    $"Le lien « {label} » ne possède pas d’attribut href.");
            }

            await link.ClickAsync();
            await Assertions.Expect(_page).ToHaveURLAsync(new Regex(Regex.Escape(href)));
        }
    }
}
