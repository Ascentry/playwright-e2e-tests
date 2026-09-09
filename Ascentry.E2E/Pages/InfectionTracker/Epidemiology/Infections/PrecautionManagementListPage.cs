using Ascentry.E2E.Contracts.Enums;
using Ascentry.E2E.Contracts.Pages;
using Ascentry.E2E.Contracts.PrecautionManagement;
using Ascentry.E2E.Core.Components;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ascentry.E2E.Pages.InfectionTracker.Epidemiology.Infections
{
    internal class PrecautionManagementListPage : EntityContextQuestionnairePageBase, IPrecautionManagementListPage
    {
        private string _pidFilter;
        private const int _precautionSampleTypeColumnPos = 2;
        private const int _precautionSpecimenCollectionDateColumnPos = 3;
        private const int _precautionSpecimenNumberColumnPos = 4;

        private IReadOnlyList<ILocator> _rows = [];

        public PrecautionManagementListPage(IPage page) : base(page)
        {
        }

        public async Task VerifyPrecautionTableAsync()
        {
            var tableContainer = Page.Locator("app-list-precautions").Locator("byg-table");
            await EnsureVisibleAsync(tableContainer, "Precaution table container is not visible");

            var spinner = new SpinnerComponent(Page);
            await spinner.VerifySpinnerAsync();
        }

        public async Task<List<PatientDetailsRow>> GetPatientPrecautionsAsync(List<string> addedEvents, bool associateSpecimen)
        {
            await VerifyNewCasesTabSelectedAsync();
            await VerifyPatientFilteredByPidAsync();
            await VerifyPatientFilteredByBirthDateAsync();

            var filteredCellContents = new List<string>() { _pidFilter };
            _rows = await GetTableRowsFilteredByContentAsync(filteredCellContents);
            var results = new List<PatientDetailsRow>(_rows.Count);

            for (int i = 0; i < _rows.Count; i++)
            {
                List<string> patientTextContents = await GetRowCellTextContentsAsync(_rows[i]);
                var pids = new List<string>() { patientTextContents[(int)PatientTextColumnEnum.Pid] };

                var patientDetailsRow = new PatientDetailsRow()
                {
                    PatientLastName = patientTextContents[(int)PatientTextColumnEnum.PatientLastName],
                    PatientFirstName = patientTextContents[(int)PatientTextColumnEnum.PatientFirstName],
                    Pids = pids,
                    BirthDate = patientTextContents[(int)PatientTextColumnEnum.BirthDate],
                    AdmissionDate = patientTextContents[(int)PatientTextColumnEnum.AdmissionDate],
                    ReleaseDate = patientTextContents[(int)PatientTextColumnEnum.ReleaseDate],
                    LastCareUnit = patientTextContents[(int)PatientTextColumnEnum.LastPatientCareUnit]
                };

                var collapseButton = _rows[i].GetByRole(AriaRole.Cell).Nth(1);
                await collapseButton.ClickAsync(); // Expand the patient precaution rows

                patientDetailsRow.Precautions = await GetPrecautionsByPatientAsync(i, addedEvents, associateSpecimen);
                results.Add(patientDetailsRow);
            }

            return results;
        }

        private async Task<List<PatientPrecautionDetailsRow>> GetPrecautionsByPatientAsync(int patientRowIndex, List<string> addedEvents, bool associateSpecimen)
        {
            var precautionTableContainer = Page.Locator("yline-subtable-container").Nth(patientRowIndex);
            var precautionRowsRoot = precautionTableContainer.Locator("table tbody");
            await precautionRowsRoot.GetByRole(AriaRole.Row).Nth(addedEvents.Count - 1)
                .WaitForAsync();
            var filterPrecautionRows = new List<ILocator>();

            foreach (var eventName in addedEvents)
            {
                // precautions are ordered by eventName then by specimen collection date
                ILocator rowsLocator;

                if (associateSpecimen)
                {
                    rowsLocator = precautionRowsRoot.GetByRole(AriaRole.Row, new LocatorGetByRoleOptions() { Name = eventName });
                }
                else
                {
                    rowsLocator = precautionRowsRoot.Locator($"tr:has(td:nth-child({_precautionSampleTypeColumnPos}):empty):has(td:nth-child({_precautionSpecimenCollectionDateColumnPos}):empty):has(td:nth-child({_precautionSpecimenNumberColumnPos}):empty)")
                        .Filter(new() { HasText = eventName });
                }

                var rows = await rowsLocator.AllAsync();
                var moreRecentPrecautionByEvent = rows[0]; // by default event associated to specimen are in top positions
                filterPrecautionRows.Add(moreRecentPrecautionByEvent);
            }

            int count = filterPrecautionRows.Count;
            var patientDetailsRows = new List<PatientPrecautionDetailsRow>(count);

            for (int j = 0; j < count; j++)
            {
                var precautionTextContents = await GetRowCellTextContentsAsync(filterPrecautionRows[j]);
                int precautionDurationColumnIndex = 0;

                var precautionDetailsRow = new PatientPrecautionDetailsRow()
                {
                    EventName = precautionTextContents[(int)PrecautionTextColumnEnum.EventName],
                    SampleType = precautionTextContents[(int)PrecautionTextColumnEnum.SampleType],
                    SpecimenCollectionDate = precautionTextContents[(int)PrecautionTextColumnEnum.SpecimenCollectionDate],
                    SpecimenNumber = precautionTextContents[(int)PrecautionTextColumnEnum.SpecimenNumber],
                    EventLocation = await GetPrecautionRowSelectValueAsync(PrecautionSelectColumnEnum.EventLocation, filterPrecautionRows[j]),
                    PrecautionType = await GetPrecautionRowSelectValueAsync(PrecautionSelectColumnEnum.PrecautionType, filterPrecautionRows[j]),
                    PrecautionStartDate = await GetPrecautionRowDateValueAsync(PrecautionDateColumnEnum.PrecautionStartDate, filterPrecautionRows[j]),
                    PrecautionDuration = await GetValueInputNumberFromCellAsync(precautionDurationColumnIndex, filterPrecautionRows[j]),
                    PrecautionEndDate = await GetPrecautionRowDateValueAsync(PrecautionDateColumnEnum.PrecautionEndDate, filterPrecautionRows[j]),
                    QuestionnaireStatus = await GetQuestionnaireStatusAsync(filterPrecautionRows[j])
                };

                patientDetailsRows.Add(precautionDetailsRow);
            }

            return patientDetailsRows;
        }

        private async Task<string> GetPrecautionRowSelectValueAsync(PrecautionSelectColumnEnum selectColumn, ILocator row)
        {
            int index = (int)selectColumn;
            List<string> selectedValues = await GetSelectedValuesFromCellAsync(index, row);

            if (selectedValues.Count == 1)
            {
                return selectedValues[0];
            }

            return string.Empty;
        }

        private async Task<string> GetPrecautionRowDateValueAsync(PrecautionDateColumnEnum dateColumn, ILocator row)
        {
            int index = (int)dateColumn;
            return await GetDateFromCellAsync(index, row);
        }

        private async Task VerifyNewCasesTabSelectedAsync()
        {
            ILocator activeTab = GetActiveTab(TranslationEnum.NewCases);
            await EnsureVisibleAsync(activeTab, "Active tab is not the new cases tab");
        }

        private async Task VerifyPatientFilteredByPidAsync()
        {
            ILocator pidInput = GetFormTextInputByLabel(TranslationEnum.Pid.ToString(), GetFiltersContainer());

            await EnsureVisibleAsync(pidInput, "Pid input is not available");
            var pid = (await pidInput.InputValueAsync()).Trim();

            if (string.IsNullOrEmpty(pid))
            {
                throw new Exception("Patient is not filtered by pid");
            }

            _pidFilter = pid;
        }

        private async Task VerifyPatientFilteredByBirthDateAsync()
        {
            ILocator birthDateInput = GetFormDateInputByLabel(TranslationEnum.DateOfBirth_1.ToString(), GetFiltersContainer());

            await EnsureVisibleAsync(birthDateInput, "Birthdate input is not available");
            var birthDate = (await birthDateInput.InputValueAsync()).Trim();

            if (string.IsNullOrEmpty(birthDate))
            {
                throw new Exception("Patient is not filtered by birthdate");
            }
        }

        private enum PatientTextColumnEnum
        {
            PatientLastName = 2,
            PatientFirstName,
            Pid = 5,
            BirthDate,
            AdmissionDate,
            ReleaseDate,
            LastPatientCareUnit
        }

        private enum PrecautionTextColumnEnum
        {
            EventName = 0,
            SampleType,
            SpecimenCollectionDate,
            SpecimenNumber
        }

        private enum PrecautionSelectColumnEnum
        {
            EventLocation = 0,
            PrecautionType
        }

        private enum PrecautionDateColumnEnum
        {
            PrecautionStartDate = 0,
            PrecautionEndDate
        }
    }
}