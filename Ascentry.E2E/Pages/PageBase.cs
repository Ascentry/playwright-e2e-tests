using Ascentry.E2E.Contracts.Enums;
using Ascentry.E2E.Core.Translations;
using Microsoft.Playwright;

namespace Ascentry.E2E.Pages
{
    internal abstract class PageBase
    {
        protected IPage Page { get; }

        internal PageBase(IPage page)
        {
            Page = page;
        }

        protected static string GetLabel(TranslationEnum key) => TranslationProvider.Get(key);
    }
}