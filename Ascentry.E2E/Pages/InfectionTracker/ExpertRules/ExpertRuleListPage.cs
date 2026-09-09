using Ascentry.E2E.Contracts.Interfaces;
using Ascentry.E2E.Enums;
using Ascentry.E2E.Navigations.Urls;
using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Ascentry.E2E.Pages.InfectionTracker.ExpertRules
{
    internal class ExpertRuleListPage : PageBase, IExpertRuleListPage
    {
        public ExpertRuleListPage(IPage page) : base(page) { }

        /// <summary>
        /// Select and open an expertrule in edit or readonly mode depending on the user rights
        /// </summary>
        /// <param name="expertruleName"></param>
        /// <returns>expert rule page</returns>
        public async Task<IExpertRulePage> OpenAsync(string expertruleName)
        {
            await Page.WaitForURLAsync(UrlProvider.Get(RouteKeyEnum.ExpertRuleList));

            // search the expert rule by name
            var searchInput = Page.Locator("byg-form-search-input[name='search'] input");
            await searchInput.FillAsync(expertruleName);

            await Page.GetByRole(AriaRole.Cell).Filter(new LocatorFilterOptions() { HasText = expertruleName.Trim() }).ClickAsync();
            return new ExpertRulePage(Page);
        }
    }
}
