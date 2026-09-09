using Ascentry.E2E.Contracts.Layout;
using Ascentry.E2E.Enums;
using Ascentry.E2E.Navigations.Urls;
using Ascentry.E2E.Pages;
using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace Ascentry.E2E.Layout
{
    internal class Header: PageBase, IHeader
    {
        private readonly ILocator _logoutButton;

        public Header(IPage page) : base(page)
        {
            _logoutButton = page.Locator("button.reset-button:has(byg-icon[icon='exit_to_app'])");
        }

        /// <summary>
        /// Log out the current user
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task LogoutUserAsync()
        {
            try
            {
                await _logoutButton.WaitForAsync(new LocatorWaitForOptions() { State = WaitForSelectorState.Visible });
                await _logoutButton.ClickAsync();

                await Page.WaitForURLAsync(UrlProvider.Get(RouteKeyEnum.Login));

                var login = new LoginPage(Page);
                await login.VerifyDisplayedAsync();

            }
            catch (Exception ex)
            {
                throw new Exception("User cannot logout", ex);
            }

        }

    }
}