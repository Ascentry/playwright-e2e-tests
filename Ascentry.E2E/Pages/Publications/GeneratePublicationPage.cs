using Ascentry.E2E.Contracts.Enums;
using Ascentry.E2E.Contracts.Interfaces;
using Ascentry.E2E.Contracts.Publications;
using Ascentry.E2E.Core.Components;
using Ascentry.E2E.Enums;
using Ascentry.E2E.Navigations.Urls;
using Microsoft.Playwright;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ascentry.E2E.Pages.Publications
{
    internal class GeneratePublicationPage : PageBase, IGeneratePublicationPage
    {
        private bool _hasSelectedPublication;

        public GeneratePublicationPage(IPage page) : base(page)
        {
        }

        /// <summary>
        /// Retrieve publication event rows
        /// </summary>
        /// <param name="eventContextType">Event context type i.e. ORU or ADT</param>
        /// <param name="publicationName">Publication name</param>
        /// <param name="eventContextIdentifier">Identifier of the event context : specimen number if context type is ORU or patient fullName if context type is ADT</param>
        /// <param name="eventName">Event name</param>
        /// <param name="startDate">Start of the period</param>
        /// <param name="endDate">End of th eperiod</param>
        /// <returns>List of the event details that correspond to the filter parameters</returns>
        public async Task<List<PublicationEventDetailsRow>> GetPublicationEventRows(PublicationEventContextTypeEnum eventContextType, string publicationName, string eventContextIdentifier, string eventName = null, string startDate = null, string endDate = null)
        {
            
            var responseTask = Page.WaitForResponseAsync(response =>
            response.Url.Contains("/api/servicebroker/publicationevent/EventsForManualyGeneratedPublication") && response.Status == 200);
            
            var spinner = new SpinnerComponent(Page);
            var selectedPublication = Page.Locator("byg-form-select[name='publicationId'] .ng-value-label");
            bool selectedValueChanged = false;            

            if (selectedPublication != null)
            {
                _hasSelectedPublication = await selectedPublication.IsVisibleAsync();
            }

            if (_hasSelectedPublication)
            {
                string selectedValue = await selectedPublication?.InnerTextAsync();
                selectedValueChanged = !selectedValue.Equals(publicationName);
            }

            if ((_hasSelectedPublication || !selectedValueChanged) && (startDate == null || endDate == null))
            {
                _hasSelectedPublication = false;
                await Page.ReloadAsync();

                await spinner.VerifySpinner();
            }

            if (!_hasSelectedPublication || selectedValueChanged)
            {
                var select = Page.Locator("byg-form-select[name='publicationId'] div.ng-select-container");
                await select.ClickAsync();
                var dropdownListOption = Page.Locator("byg-form-select[name='publicationId'] div.custom-option").Filter(new() { HasTextString = publicationName });

                await Assertions.Expect(dropdownListOption).ToBeVisibleAsync();
                await dropdownListOption.ClickAsync();
                await responseTask;
                await spinner.VerifySpinner();
            }

            if (startDate != null)
            {
                var startDateInput = Page.Locator("byg-form-period byg-form-date input").Nth(0);
                await startDateInput.FillAsync(startDate);
                await Page.Keyboard.PressAsync("Enter");
                await responseTask;
                await spinner.VerifySpinner();
            }

            if (endDate != null)
            {
                var endDateInput = Page.Locator("byg-form-period byg-form-date input").Nth(1);
                await endDateInput.FillAsync(endDate);
                await Page.Keyboard.PressAsync("Enter");
                await responseTask;
                await spinner.VerifySpinner();

                // To force the datepicker to close and launch the request
                await Page.Keyboard.PressAsync("Tab");
            }

            await spinner.VerifySpinner();

            return await BuildRows(eventContextType, eventContextIdentifier, eventName);
        }


        private async Task<List<PublicationEventDetailsRow>> BuildRows(PublicationEventContextTypeEnum eventContextType, string eventContextIdentifier, string eventName = null)
        {
            ILocator rowElements = Page.Locator("byg-table.events-table tbody > tr");
            int rowCount = await rowElements.CountAsync();
            var eventDetailsRows = new List<ILocator>(rowCount);

            // Wait for the results to load in the table
            await rowElements.Last.WaitForAsync();

            if (eventContextType == PublicationEventContextTypeEnum.ORU)
            {
                var filteredElements = rowElements.Filter(new LocatorFilterOptions()
                {
                    HasTextString = eventContextIdentifier
                });

                int filteredCount = await filteredElements.CountAsync();

                for (int i = 0; i < filteredCount; i++)
                {
                    var row = filteredElements.Nth(i);
                    eventDetailsRows.Add(row);
                }
            }
            else if (eventContextType == PublicationEventContextTypeEnum.ADT)
            {
                var filteredElements = rowElements.Filter(new LocatorFilterOptions()
                {
                    HasTextString = eventContextIdentifier
                });

                int filteredCount = await filteredElements.CountAsync();

                for (int i = 0; i < filteredCount; i++)
                {
                    var row = rowElements.Nth(i);
                    var patientContent = await row.Locator("td.row-detail").Nth((int)PublicationEventColumnEnum.Patient).InnerTextAsync();
                    patientContent = patientContent.Trim();

                    if (string.IsNullOrEmpty(patientContent))
                    {
                        eventDetailsRows.Add(row);
                    }
                }
            }

            if (!string.IsNullOrEmpty(eventName) && eventDetailsRows.Count != 0)
            {
                var copy = new List<ILocator>(eventDetailsRows);
                eventDetailsRows = new List<ILocator>(copy.Count);

                for (int i = 0; i < copy.Count; i++)
                {
                    var row = rowElements.Nth(i);
                    var eventContent = await row.Locator("td.row-detail").Nth((int)PublicationEventColumnEnum.EventName).InnerTextAsync();
                    eventContent = eventContent.Trim();

                    if (eventContent.Equals(eventName))
                    {
                        eventDetailsRows.Add(row);
                    }
                }
            }

            if (eventDetailsRows.Count == 0)
            {
                return new List<PublicationEventDetailsRow>();
            }

            var eventDetailsRowResults = new List<PublicationEventDetailsRow>(eventDetailsRows.Count);

            for (int i = 0; i < eventDetailsRows.Count; i++)
            {
                ILocator row = eventDetailsRows[i];
                var cellContents = await row.Locator("td.row-detail").AllInnerTextsAsync();
                var rowDetail = new PublicationEventDetailsRow()
                {
                    SpecimenNumber = cellContents[(int)PublicationEventColumnEnum.SpecimenNumber],
                    CollectionDate = cellContents[(int)PublicationEventColumnEnum.CollectionDate],
                    Patient = cellContents[(int)PublicationEventColumnEnum.Patient],
                    EventName = cellContents[(int)PublicationEventColumnEnum.EventName],
                    EventEstablishment = cellContents[(int)PublicationEventColumnEnum.EventEstablishment],
                    EventDepartment = cellContents[(int)PublicationEventColumnEnum.EventDepartment],
                    EventCareUnit = cellContents[(int)PublicationEventColumnEnum.EventCareUnit]
                };

                eventDetailsRowResults.Add(rowDetail);
            }

            return eventDetailsRowResults;
        }

        private enum PublicationEventColumnEnum
        {
            SpecimenNumber = 0,
            CollectionDate,
            Patient,
            EventName,
            EventEstablishment,
            EventDepartment,
            EventCareUnit
        }
    }
}
