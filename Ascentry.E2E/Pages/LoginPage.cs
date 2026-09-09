using Ascentry.E2E.Contracts.Interfaces;
using Ascentry.E2E.Enums;
using Ascentry.E2E.Navigations.Urls;
using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace Ascentry.E2E.Pages
{
    internal class LoginPage: PageBase, ILoginPage
    {
        private readonly ILocator _loginInput;
        private readonly ILocator _passwordInput;
        private readonly ILocator _loginButton;
        private readonly ILocator _welcomeText;

        public LoginPage(IPage page) : base(page)
        {

            _loginInput = page.Locator("byg-form-text[name='login'] input");
            _passwordInput = page.Locator("byg-form-text[name='password'] input");
            _loginButton = page.Locator("byg-button:has(input[type='submit'])");
            _welcomeText = page.GetByText("Bienvenue sur votre Dashboard");
        }


        public async Task VerifyDisplayedAsync()
        {
            await Assertions.Expect(_loginInput)
                .ToBeVisibleAsync();

            await Assertions.Expect(_passwordInput)
                .ToBeVisibleAsync();

            await Assertions.Expect(_loginButton)
                .ToBeVisibleAsync();
        }

        public async Task VerifyWelcomeMessageAsync()
        {
            await Assertions.Expect(_welcomeText).ToBeVisibleAsync();
        }

        public async Task LoginUserAsync(string username, string password)
        {
            try
            {
                await Assertions.Expect(_loginInput).ToBeVisibleAsync();
                await Assertions.Expect(_passwordInput).ToBeVisibleAsync();

                await _loginInput.FillAsync(username);
                await _passwordInput.FillAsync(password);

                await _loginButton.ClickAsync();
                await Page.WaitForURLAsync(UrlProvider.Get(RouteKeyEnum.Home));

                await Assertions.Expect(_welcomeText).ToBeVisibleAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Login failed for user {username}.", ex);
            }

        }
    }
}
