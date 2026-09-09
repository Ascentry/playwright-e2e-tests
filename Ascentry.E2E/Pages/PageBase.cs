using Ascentry.E2E.Contracts.Enums;
using Ascentry.E2E.Core.Translations;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ascentry.E2E.Pages
{
    internal abstract class PageBase
    {
        protected IPage Page { get; set; }

        internal PageBase(IPage page)
        {
            Page = page;
        }

        protected static string GetLabel(TranslationEnum key) => TranslationProvider.Get(key);

        protected static string GetLabelWithoutParameters(TranslationEnum translation)
        {
            return Regex.Replace(GetLabel(translation), @"\{\d+\}", "")
                .Trim();
        }


        protected static async Task SelectDropDownListOptionsAsync(ILocator select, List<string> values)
        {
            await select.ClickAsync();
            ILocator options = select.Locator("div.custom-option.option-visible");

            foreach (var value in values)
            {
                var option = options.Filter(new() { HasText = value });
                await option.ClickAsync();
            }
        }

        protected async Task<List<string>> GetSelectedValuesFromCellAsync(int rowSelectOrder, ILocator parent = null)
        {
            string selector = "byg-select";
            ILocator root;

            if (parent != null)
            {
                root = parent.Locator(selector).Nth(rowSelectOrder).Locator("div.ng-has-value div.ng-value");
            }
            else
            {
                root = Page.Locator(selector).Nth(rowSelectOrder).Locator("div.ng-has-value div.ng-value");
            }

            var selectedValues = await root.AllInnerTextsAsync();

            return selectedValues.ToList();
        }

        protected ILocator GetFiltersContainer()
        {
            return Page.Locator("byg-filters");
        }

        protected ILocator GetFormInputByName(string name, ILocator parent = null)
        {
            string selector = $"byg-form-text[name='{name}'] input";
            return (parent != null) ? parent.Locator(selector) : Page.Locator(selector);
        }

        protected ILocator GetFormTextInputByLabel(string label, ILocator parent = null)
        {
            string selector = $"byg-form-text[label='{label}'] input";
            return (parent != null) ? parent.Locator(selector) : Page.Locator(selector);
        }

        protected ILocator GetFormDateInputByLabel(string label, ILocator parent = null)
        {
            string selector = $"byg-form-date[label='{label}'] input";
            return (parent != null) ? parent.Locator(selector) : Page.Locator(selector);
        }

        protected Task<string> GetValueInputNumberFromCellAsync(int dateOrder, ILocator parent = null)
        {
            string selector = $"byg-number input[type='number']";
            ILocator root;

            if (parent != null)
            {
                root = parent.Locator(selector).Nth(dateOrder);
            }
            else
            {
                root = Page.Locator(selector).Nth(dateOrder);
            }

            return root.InputValueAsync();
        }

        protected async Task<string> GetDateFromCellAsync(int dateOrder, ILocator parent = null)
        {
            string selector = $"byg-date input";
            ILocator root;

            if (parent != null)
            {
                root = parent.Locator(selector).Nth(dateOrder);
            }
            else
            {
                root = Page.Locator(selector).Nth(dateOrder);
            }

            var date = await root.InputValueAsync();
            return date;
        }

        protected ILocator GetFormSelectByName(string name, ILocator parent = null)
        {
            string selector = $"byg-form-select[name='{name}']";
            return (parent != null) ? parent.Locator(selector) : Page.Locator(selector);
        }

        protected ILocator GetButton(TranslationEnum translation, ILocator parent = null, bool? disabled = null)
        {
            string label = GetLabel(translation);
            string selector = $"byg-button:has(label:has-text('{label}'))";

            if (disabled == true)
            {
                selector += "[disabled='true']";
            }

            return (parent != null) ? parent.Locator(selector) : Page.Locator(selector);
        }

        protected ILocator GetRadio(TranslationEnum translation, ILocator parent = null, bool? translationContainesParams = null)
        {
            string radioName;

            if (translationContainesParams == true)
            {
                radioName = GetLabelWithoutParameters(translation);
            }
            else
            {
                radioName = GetLabel(translation);
            }

            if (parent != null)
            {
                return parent.GetByRole(AriaRole.Radio, new LocatorGetByRoleOptions() { Name = GetLabel(translation) });
            }

            return Page.GetByRole(AriaRole.Radio, new PageGetByRoleOptions() { Name = radioName });
        }

        protected ILocator GetCheckedRadio(TranslationEnum translation, ILocator parent = null, bool? translationContainesParams = null)
        {
            string selector = "span.ant-radio-checked";
            string radioName;

            if (translationContainesParams == true)
            {
                radioName = GetLabelWithoutParameters(translation);
            }
            else
            {
                radioName = GetLabel(translation);
            }

            ILocator root;

            if (parent != null)
            {
                root = parent.Locator(selector);
            }
            else
            {
                root = Page.Locator(selector);
            }

            return root.GetByRole(AriaRole.Radio, new LocatorGetByRoleOptions() { Name = radioName });
        }

        protected ILocator GetDialog(ILocator parent)
        {
            string selector = "byg-dialog";
            return (parent != null) ? parent.Locator(selector) : Page.Locator(selector);
        }


        protected ILocator GetActiveTab(TranslationEnum translation, ILocator parent = null, bool? translationContainesParams = null)
        {
            string tabName;
            string selector = "byg-tabs li.active";

            if (translationContainesParams == true)
            {
                tabName = GetLabelWithoutParameters(translation);
            }
            else
            {
                tabName = GetLabel(translation);
            }

            ILocator root = (parent != null) ? parent.Locator(selector) : Page.Locator(selector);

            return root.Filter(new LocatorFilterOptions() { HasText = tabName });
        }

        protected async Task<IReadOnlyList<ILocator>> GetTableRowsFilteredByContentAsync(List<string> cellTextContents, ILocator parent = null)
        {
            List<ILocator> rows = [];
            var selector = "byg-table";
            ILocator root;

            if (parent != null)
            {
                root = parent.Locator(selector);
            }
            else
            {
                root = Page.Locator(selector);
            }

            for (int i = 0; i < cellTextContents.Count; i++)
            {
                var filteredRows = await root.GetByRole(AriaRole.Row, new LocatorGetByRoleOptions() { Name = cellTextContents[i] }).AllAsync();
                rows.AddRange(filteredRows);
            }

            return rows;
        }


        protected static async Task<IReadOnlyList<ILocator>> GetTableRowsAsync(ILocator parent)
        {
            return await parent.GetByRole(AriaRole.Row).AllAsync();
        }


        protected static async Task<List<string>> GetRowCellTextContentsAsync(ILocator row)
        {
            IReadOnlyList<ILocator> cells = await row.GetByRole(AriaRole.Cell).AllAsync();
            var textContents = new List<string>(cells.Count);

            for (int i = 0; i < cells.Count; i++)
            {
                var currentText = (await cells[i].TextContentAsync()).Trim();
                textContents.Add(currentText);
            }

            return textContents;
        }

        protected async Task<ILocator> GetCollapseButtonAsync(ILocator parent = null)
        {
            string selector = "byg-collapse";
            ILocator root = (parent != null) ? parent.Locator(selector) : Page.Locator(selector);

            var buttons = await root.GetByRole(AriaRole.Button).AllAsync();

            return buttons.First();
        }

        protected ILocator GetIcon(string iconName, ILocator parent = null)
        {
            string selector = $"byg-icon[icon='{iconName}']";
            return (parent != null) ? parent.Locator(selector) : Page.Locator(selector);
        }

        protected ILocator GetIconBySvgGroupId(string iconName, ILocator parent = null)
        {
            string selector = $"byg-icon g[id='{iconName}']";
            return (parent != null) ? parent.Locator(selector) : Page.Locator(selector);
        }

        protected ILocator GetLink(string name, ILocator parent = null)
        {
            if (parent != null)
            {
                return parent.GetByRole(AriaRole.Link, new LocatorGetByRoleOptions() { Name = name });
            }

            return Page.GetByRole(AriaRole.Link, new PageGetByRoleOptions() { Name = name });
        }


        protected static async Task EnsureVisibleAsync(ILocator locator, string errorMessage)
        {
            try
            {
                await Assertions.Expect(locator).ToBeVisibleAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(errorMessage, ex);
            }
        }

        protected static async Task EnsureNotVisibleAsync(ILocator locator, string errorMessage)
        {
            try
            {
                await Assertions.Expect(locator).Not.ToBeVisibleAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(errorMessage, ex);
            }
        }
    }
}