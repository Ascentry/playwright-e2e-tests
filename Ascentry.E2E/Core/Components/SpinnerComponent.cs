using Microsoft.Playwright;
using System.Threading.Tasks;

namespace Ascentry.E2E.Core.Components
{
    internal class SpinnerComponent
    {
        private readonly IPage _page;

        public SpinnerComponent(IPage page)
        {
            _page = page;
        }

        public async Task VerifySpinnerAsync()
        {
            var spinner = _page.Locator("byg-spin");

            if (await spinner.IsVisibleAsync())
            {
                await Assertions.Expect(spinner).ToBeHiddenAsync();
            }
        }
    }
}
