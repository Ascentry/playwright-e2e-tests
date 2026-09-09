using Ascentry.E2E.Contracts.Enums;
using Ascentry.E2E.Contracts.InfectionMonitoring;
using Ascentry.E2E.Contracts.Pages;
using Ascentry.E2E.Core.Components;
using Ascentry.E2E.Pages.InfectionTracker.PatientRecord;
using Microsoft.Playwright;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ascentry.E2E.Pages.InfectionTracker.Epidemiology.Infections
{
    internal class InfectionMonitoringListPage : EntityContextQuestionnairePageBase, IInfectionMonitoringListPage
    {
        private readonly SpinnerComponent _spinner;
        private IReadOnlyList<ILocator> _rows = [];

        public InfectionMonitoringListPage(IPage page) : base(page)
        {
            _spinner = new SpinnerComponent(page);
        }

        public async Task<List<InfectionDetailsRow>> GetInfectionsAsync(InfectionFilter filter)
        {
            await VerifyPendingTabSelectedAsync();
            List<InfectionDetailsRow> infectionDetailsRows = await SearchInfectionAsync(filter);
            return infectionDetailsRows;
        }

        private async Task VerifyPendingTabSelectedAsync()
        {
            ILocator activeTab = GetActiveTab(TranslationEnum.InProgress);
            await EnsureVisibleAsync(activeTab, "Active tab is not the pending tab.");
        }


        private async Task<List<InfectionDetailsRow>> SearchInfectionAsync(InfectionFilter filter)
        {
            ILocator nameInput = GetFormTextInputByLabel(TranslationEnum.Name.ToString(), GetFiltersContainer());
            await EnsureVisibleAsync(nameInput, "Name input cannot be filled.");

            if (!string.IsNullOrEmpty(filter.PatientLastName))
            {
                await nameInput.FillAsync(filter.PatientLastName);
            }

            ILocator applyButton = GetButton(TranslationEnum.Apply, GetFiltersContainer());
            await applyButton.ClickAsync();

            await _spinner.VerifySpinnerAsync();

            var filteredCellContents = new List<string>() { filter.PatientLastName };
            _rows = await GetTableRowsFilteredByContentAsync(filteredCellContents);
            var results = new List<InfectionDetailsRow>(_rows.Count);

            for (int i = 0; i < _rows.Count; i++)
            {
                List<string> textContents = await GetRowCellTextContentsAsync(_rows[i]);
                var emptyCommentIcon = GetIconBySvgGroupId("comment_empty", _rows[i]);

                var infectionDetailRow = new InfectionDetailsRow()
                {
                    PatientLastName = textContents[(int)InfectionTextColumnEnum.PatientLastName],
                    PatientFirstName = textContents[(int)InfectionTextColumnEnum.PatientFirstName],
                    SampleType = textContents[(int)InfectionTextColumnEnum.SampleType],
                    SpecimenCollectionDate = textContents[(int)InfectionTextColumnEnum.SpecimenCollectionDate],
                    CareUnit = textContents[(int)InfectionTextColumnEnum.CareUnit],
                    InfectionType = await GetInfectionRowSelectValueAsync(InfectionSelectColumnEnum.InfectionType, _rows[i]),
                    InfectionStatus = await GetInfectionRowSelectValueAsync(InfectionSelectColumnEnum.InfectionStatus, _rows[i]),
                    InfectionQualification = await GetInfectionRowSelectValueAsync(InfectionSelectColumnEnum.InfectionQualification, _rows[i]),
                    HasEmptyComments = await emptyCommentIcon.CountAsync() > 0,
                    QuestionnaireStatus = await GetQuestionnaireStatusAsync(_rows[i])
                };

                results.Add(infectionDetailRow);
            }

            return results;
        }

        public async Task<IPatientRecordPage> GoToPatientRecordAsync(string patientLastName, int rowIndex)
        {
            var patientRecordLink = GetLink(patientLastName, _rows[rowIndex]);
            await patientRecordLink.ClickAsync();
            var newTab = await Page.Context.WaitForPageAsync();
            var patientRecordPage = new PatientRecordPage(newTab);

            var spinner = new SpinnerComponent(newTab);
            await spinner.VerifySpinnerAsync();
            await patientRecordPage.VerifyPatientNameAsync();
            return patientRecordPage;
        }

        private async Task<string> GetInfectionRowSelectValueAsync(InfectionSelectColumnEnum selectColumn, ILocator row)
        {
            int index = (int)selectColumn;
            List<string> selectedValues = await GetSelectedValuesFromCellAsync(index, row);

            if (selectedValues.Count == 1)
            {
                return selectedValues[0];
            }

            return string.Empty;
        }

        private enum InfectionTextColumnEnum
        {
            PatientLastName = 1,
            PatientFirstName,
            SampleType,
            SpecimenCollectionDate,
            CareUnit,
            InfectionType,
            InfectionStatus,
            InfectionQualification,
            HasComments,
            QuestionnaireStatus
        }

        private enum InfectionSelectColumnEnum
        {
            InfectionType = 0,
            InfectionStatus,
            InfectionQualification
        }
    }
}
