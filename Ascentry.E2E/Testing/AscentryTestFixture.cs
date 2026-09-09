using Ascentry.E2E.Configurations;
using Ascentry.E2E.Contracts.Interfaces;
using Ascentry.E2E.Contracts.Layout;
using Ascentry.E2E.Enums;
using Ascentry.E2E.Navigations.Menus;
using Ascentry.E2E.Navigations.Urls;
using Ascentry.E2E.Pages;
using Ascentry.E2E.Pages.ExpertRules;
using Ascentry.E2E.Pages.Publications;
using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Ascentry.E2E.Testing
{
    public class AscentryFixture : IAsyncLifetime
    {
        private const int DefaultSlowMo = 300;
        private IPlaywright _playwright;
        private IPage _page;
        private IBrowserContext _context;
        private IBrowser _browser;
        public ILoginPage LoginPage { get; private set; }
        public IHeader Header { get; private set; }
        public INavigationMenu NavigationMenu { get; private set; }
        public IGeneratePublicationPage GeneratePublicationPage { get; private set; }
        public IExpertRuleListPage ExpertRuleListPage { get; private set; }
        public IExpertRulePage ExpertRulePage { get; private set; }

        protected virtual bool HeadLess
        {
            get
            {
                var value = GetSetting("PLAYWRIGHT_HEADLESS", TestConfiguration.Settings.HeadLess);

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
                var value = GetSetting("PLAYWRIGHT_SLOWMO", TestConfiguration.Settings.SlowMo);

                if (int.TryParse(value, out int result))
                {
                    return result;
                }

                return DefaultSlowMo;
            }
        }

        protected virtual string UserName => GetSetting("PLAYWRIGHT_USERNAME", TestConfiguration.Settings.UserName);

        private string Password => GetSetting("PLAYWRIGHT_PASSWORD", TestConfiguration.Settings.Password);

        /// <summary>
        /// Appelé une seule fois lors de l'initialisation de la fixture
        /// </summary>
        public virtual async ValueTask InitializeAsync()
        {
            var configuration = new ConfigurationBuilder()
            .AddJsonFile("configuration.json", optional: true)
            .AddUserSecrets<AscentryFixture>(optional: true)
            .AddEnvironmentVariables()
            .Build();

            TestConfiguration.Initialize(configuration);

            _playwright = await Playwright.CreateAsync();

            _browser = await _playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions()
                {
                    Headless = HeadLess,
                    SlowMo = SlowMoDurationMs
                });

            _context = await _browser.NewContextAsync();

            _page = await _context.NewPageAsync();

            LoginPage = new LoginPage(_page);
            Header = new Layout.Header(_page);
            NavigationMenu = new NavigationMenu(_page);
            GeneratePublicationPage = new GeneratePublicationPage(_page);
            ExpertRuleListPage = new ExpertRuleListPage(_page);
            ExpertRulePage = new ExpertRulePage(_page);

            await LoginUser();
        }

        private async Task LoginUser()
        {
            var url = UrlProvider.Get(RouteKeyEnum.Login);

            await _page.GotoAsync(url);
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

            await LoginPage.LoginUserAsync(UserName, Password);
        }

        /// <summary>
        /// This method is called when the fixture is disposed.
        /// </summary>
        public virtual async ValueTask DisposeAsync()
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

        private static string GetSetting(
            string envVar,
            string defaultValue)
        {
            var value = Environment.GetEnvironmentVariable(envVar);

            if (string.IsNullOrEmpty(value))
            {
                value = defaultValue;
            }

            return value;
        }
    }
}