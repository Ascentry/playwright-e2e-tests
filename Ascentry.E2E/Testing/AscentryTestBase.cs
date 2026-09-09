using Ascentry.E2E.Configurations;
using Ascentry.E2E.Contracts.Interfaces;
using Ascentry.E2E.Contracts.Layout;
using Ascentry.E2E.Enums;
using Ascentry.E2E.Navigations.Menus;
using Ascentry.E2E.Navigations.Urls;
using Ascentry.E2E.Pages;
using Ascentry.E2E.Pages.ExpertRules;
using Ascentry.E2E.Pages.Publications;
using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Ascentry.E2E.Testing
{

    public abstract class AscentryTestBase : IAsyncLifetime
    {
        private IPlaywright _playwright;
        private IPage _page;
        private IBrowserContext _context;
        private IBrowser _browser;
        private const int DefaultSlowMo = 300;
        protected ILoginPage LoginPage => new LoginPage(_page) as ILoginPage;
        protected IHeader Header => new Layout.Header(_page) as IHeader;
        protected INavigationMenu NavigationMenu => new NavigationMenu(_page) as INavigationMenu;
        protected IGeneratePublicationPage GeneratePublicationPage => new GeneratePublicationPage(_page) as IGeneratePublicationPage;
        protected IExpertRuleListPage ExpertRuleListPage => new ExpertRuleListPage(_page) as IExpertRuleListPage;

        protected virtual bool HeadLess
        {
            get
            {
                var value = Environment.GetEnvironmentVariable("PLAYWRIGHT_HEADLESS");

                if (string.IsNullOrEmpty(value))
                {
                    value = TestConfiguration.Settings.HeadLess;
                }

                if (bool.TryParse(value, out bool result))
                {
                    return result;
                }

                return false;
            }
        }

        protected virtual int SlowMoDurationMs
        {
            get
            {
                var value = Environment.GetEnvironmentVariable("PLAYWRIGHT_SLOWMO");

                if (string.IsNullOrEmpty(value))
                {
                    value = TestConfiguration.Settings.SlowMo;
                }

                if (int.TryParse(value, out int result))
                {
                    return result;
                }

                return DefaultSlowMo;
            }
        }

        protected virtual string UserName
        {
            get
            {
                var value = Environment.GetEnvironmentVariable("PLAYWRIGHT_USERNAME");

                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }

                return TestConfiguration.Settings.UserName;
            }
        }

        protected virtual string Password
        {
            get
            {
                var value = Environment.GetEnvironmentVariable("PLAYWRIGHT_PASSWORD");

                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }

                return TestConfiguration.Settings.Password;
            }
        }

        /// <summary>
        /// This method is called before each test (set up step)
        /// </summary>
        public async ValueTask InitializeAsync()
        {
            _playwright = await Microsoft.Playwright.Playwright.CreateAsync();

            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions()
            {
                Headless = HeadLess,
                SlowMo = SlowMoDurationMs
            });

            _context = await _browser.NewContextAsync();
            _page = await _context.NewPageAsync();
            await LoginUser();
        }

        private async Task LoginUser()
        {
            var url = UrlProvider.Get(RouteKeyEnum.Login);
            await _page.GotoAsync(url);
            await _page.WaitForURLAsync(url);
            var login = new LoginPage(_page);
            await login.LoginUserAsync(UserName, Password);
        }

        /// <summary>
        /// This method is called after each test (tear down step)
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (_context != null)
            {
                await _context.DisposeAsync();
            }

            if (_browser != null)
            {
                await _browser.DisposeAsync();
            }

            if (_playwright != null)
            {
                _playwright.Dispose();
            }
        }
    }
}