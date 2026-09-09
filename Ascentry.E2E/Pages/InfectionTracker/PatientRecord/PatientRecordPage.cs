using Ascentry.E2E.Contracts.Enums;
using Ascentry.E2E.Contracts.Pages;
using Ascentry.E2E.Contracts.PatientRecord;
using Ascentry.E2E.Core.Components;
using Ascentry.E2E.Pages.InfectionTracker.Epidemiology.Infections;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ascentry.E2E.Pages.InfectionTracker.PatientRecord
{
    internal class PatientRecordPage : PageBase, IPatientRecordPage
    {

        private ILocator AddToPatientMonitoringDialog => GetDialog(Page.Locator("app-add-patient-to-monitoring"));
        private ILocator EventSelect => GetFormSelectByName("eventSettings", AddToPatientMonitoringDialog);
        private ILocator PatientNameContainer => Page.Locator("infectio-lab-patient-header div#fullName");
        private ILocator SpecimenDetailsContainer => Page.Locator("infectio-lab-specimen-details");
        private readonly SpinnerComponent _spinner;

        public PatientRecordPage(IPage page) : base(page)
        {
            _spinner = new SpinnerComponent(page);
        }

        /// <summary>
        /// Search patients according to the filters
        /// </summary>
        /// <param name="filter">Filter to search the patient (e.g. name)</param>
        /// <exception cref="Exception">Throws exception if patient cannot be found</exception>
        public async Task SearchPatientAsync(PatientRecordFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.PatientLastName))
            {
                var nameInput = GetFormInputByName("name");
                await nameInput.WaitForAsync(new LocatorWaitForOptions() { State = WaitForSelectorState.Visible });
                await nameInput.FillAsync(filter.PatientLastName.Trim());
            }

            var applyButton = GetButton(TranslationEnum.Apply);
            await applyButton.ClickAsync();

            await Assertions.Expect(PatientNameContainer).ToBeVisibleAsync();
            string patientName = await PatientNameContainer.InnerTextAsync();

            if (string.IsNullOrEmpty(patientName))
            {
                throw new Exception($"The patient cannot be found. Name: {filter.PatientLastName}");
            }
        }

        public async Task VerifyPatientNameAsync()
        {
            await EnsureVisibleAsync(PatientNameContainer, "Patient name container is not visible");
        }

        /// <summary>
        /// Retrieve the details fro the default selected patient
        /// </summary>
        public async Task<PatientDetails> GetSelectedPatientDetailsAsync()
        {
            ILocator patientHeaderContainer = Page.Locator("infectio-lab-patient-header");
            await VerifyPatientNameAsync();

            string fullName = await patientHeaderContainer.Locator("div#fullName").TextContentAsync();
            var fullNameParts = fullName.Split(" ");

            string birthDateDetails = await patientHeaderContainer.Locator("div.birthDate").TextContentAsync();
            var birthDateDetailsParts = birthDateDetails.Split(" ");

            string pid1Label = $"{GetLabel(TranslationEnum.Pid)} 1";
            ILocator pid1LabelContainer = patientHeaderContainer.GetByText(pid1Label);
            int pid1Index = 1;
            string pid1 = await pid1LabelContainer.Locator($"xpath=following-sibling::span[{pid1Index}]").TextContentAsync();

            var patientDetails = new PatientDetails()
            {
                PatientLastName = fullNameParts[0],
                PatientFirstName = fullNameParts[1],
                PatientBirthDate = birthDateDetailsParts[0],
                Pids = new List<string> { pid1 }
            };

            return patientDetails;
        }

        /// <summary>
        /// Retrieve the etails for  the default selected patient stay
        /// </summary>
        public async Task<PatientStayDetails> GetSelectedPatientStayDetailsAsync()
        {
            ILocator staySummaryContainer = Page.Locator("div#stay-summary-vertical");
            ILocator patientStayPeriodLabel = staySummaryContainer.GetByText(GetLabel(TranslationEnum.Stay));
            int stayPeriodIndex = 1;
            string stayPeriod = await patientStayPeriodLabel.Locator($"xpath=following-sibling::div[{stayPeriodIndex}]").TextContentAsync();
            var stayPeriodParts = stayPeriod.Split(" ");

            ILocator collapseButton = await GetCollapseButtonAsync(SpecimenDetailsContainer);
            await collapseButton.ClickAsync();

            string patientStayCareUnitRaw = await SpecimenDetailsContainer.Locator("td").Nth(2).TextContentAsync();
            var patientStayCareUnitParts = patientStayCareUnitRaw.Split(" : ");

            string patientStayCareUnit = patientStayCareUnitParts[1]
                .Replace(GetLabel(TranslationEnum.Discharged), "")
                .Replace("()", "")
                .Trim();

            var releaseDate = stayPeriodParts[2].Replace("…", "");

            var patientStayDetails = new PatientStayDetails()
            {
                AdmissionDate = stayPeriodParts[0],
                ReleaseDate = releaseDate,
                PatientStayCareUnit = patientStayCareUnit
            };

            return patientStayDetails;
        }

        /// <summary>
        /// Retrieve the details for the default selected patient specimen
        /// </summary>
        public async Task<PatientSpecimenDetails> GetSelectedSpecimenDetailsAsync()
        {
            var specimenHeaderContent = await SpecimenDetailsContainer.Locator("section[header] span").TextContentAsync();
            var headerParts = specimenHeaderContent.Split(" - ");

            var specimenDetails = new PatientSpecimenDetails()
            {
                SpecimenCollectionDate = headerParts[0],
                SampleType = headerParts[1],
                SpecimenNumber = headerParts[2],
            };

            ILocator collapseButton = await GetCollapseButtonAsync(SpecimenDetailsContainer);
            await collapseButton.ClickAsync();

            var detailRows = await SpecimenDetailsContainer.Locator("section[detail] tr").AllAsync();
            var firstRowCellContents = await detailRows[0].Locator("td").AllInnerTextsAsync();
            int careUnitPos = 1;
            int careUnitTextPos = 1;
            specimenDetails.SpecimenCareUnit = firstRowCellContents[careUnitPos].Split(" : ")[careUnitTextPos].Trim();

            return specimenDetails;
        }

        /// <summary>
        /// Add the selected patient to the infection monitoring
        /// </summary>
        /// <param name="associateSpecimen">Indicate to add the patient to monitoring with or without a specimen</param>
        public async Task<IInfectionMonitoringListPage> AddSelectedPatientToInfectionMonitoringAsync(bool associateSpecimen)
        {
            await SharedAddToMonotoringStepsAsync();
            await SelectInfectionMonitoringAsync();
            await VerifyEventSelectDisabled();

            if (!associateSpecimen)
            {
                await SelectWithoutSpecimenRadio();
            }

            ILocator validateButton = GetButton(TranslationEnum.Validate, AddToPatientMonitoringDialog);
            await validateButton.ClickAsync();

            return new InfectionMonitoringListPage(Page);
        }

        /// <summary>
        /// Add the selected patient to the precaution management
        /// </summary>
        /// <param name="addedEvents">Events to assoaciate to patient in the precaution management</param>
        /// <param name="associateSpecimen">Indicate to add the patient to monitoring with or without a specimen</param>
        /// <exception cref="Exception"></exception>
        public async Task<IPrecautionManagementListPage> AddSelectedPatientToPrecautionManagementAsync(List<string> addedEvents, bool associateSpecimen)
        {
            await SharedAddToMonotoringStepsAsync();

            if (addedEvents.Count == 0)
            {
                throw new Exception("In Precaution management mode, the selection of at least one event is mandatory for adding a patient to monitoring.");
            }

            await SelectDropDownListOptionsAsync(EventSelect, addedEvents);

            if (!associateSpecimen)
            {
                // To force the select to close in order to view the radio button
                await Page.Keyboard.PressAsync("Tab");
                await SelectWithoutSpecimenRadio();
            }

            ILocator validateButton = GetButton(TranslationEnum.Validate, AddToPatientMonitoringDialog);
            await validateButton.ClickAsync();

            var precautionListPage = new PrecautionManagementListPage(Page);
            await precautionListPage.VerifyPrecautionTableAsync();

            return precautionListPage;
        }

        /// <summary>
        /// Indicate if the selected patient has precautions by verifying that the precaution icon is present
        /// </summary>
        public async Task<bool> PatientPrecautionsAsync()
        {
            await _spinner.VerifySpinnerAsync();
            ILocator infectionIcon = GetIcon("precaution_management");
            return await infectionIcon.CountAsync() > 0;
        }

        /// <summary>
        /// Indicate if the selected patient has infections by verifying that the infection icon is present
        /// </summary>
        public async Task<bool> PatientHasInfectionsAsync()
        {
            await _spinner.VerifySpinnerAsync();
            ILocator infectionIcon = GetIcon("ias");
            return await infectionIcon.CountAsync() > 0;
        }

        private async Task SelectWithoutSpecimenRadio()
        {
            ILocator addWithoutSpecimenRadio = GetRadio(TranslationEnum.WithoutSample_1, AddToPatientMonitoringDialog);
            await addWithoutSpecimenRadio.ClickAsync();
        }

        private async Task SharedAddToMonotoringStepsAsync()
        {
            await OpenAddtoMonitoringDialog();
            await VerifyPrecautionDefaultMonitoringAsync();
            await VerifyEventSelectionAsync();
            await VerifyDefaultAssociationToSpecimen();
            await VerifyValidateButtonAsync();
        }

        private async Task OpenAddtoMonitoringDialog()
        {
            var infectionTrackingButton = GetButton(TranslationEnum.AddToTheMonitoring);
            await Assertions.Expect(infectionTrackingButton).ToBeEnabledAsync();
            await infectionTrackingButton.ClickAsync();

            await Assertions.Expect(AddToPatientMonitoringDialog).ToBeVisibleAsync();
        }

        private async Task VerifyPrecautionDefaultMonitoringAsync()
        {
            ILocator precautionManagmentRadio = GetCheckedRadio(TranslationEnum.PrecautionManagement, AddToPatientMonitoringDialog);
            await EnsureVisibleAsync(precautionManagmentRadio, "Precaution management is not the default mode for adding a patient to monitoring.");
        }

        private async Task VerifyEventSelectionAsync()
        {
            ILocator selectedValueLabel = EventSelect.Locator("byg-select span.ng-value-label");
            await EnsureNotVisibleAsync(selectedValueLabel, "Events have been already selected.");
        }

        private async Task VerifyDefaultAssociationToSpecimen()
        {
            ILocator addWithSpecimenRadio = GetCheckedRadio(TranslationEnum.WithSampleNumber, AddToPatientMonitoringDialog, true);
            await EnsureVisibleAsync(addWithSpecimenRadio, "Association with the specimen is not enabled");
        }

        private async Task VerifyValidateButtonAsync()
        {
            ILocator validateButton = GetButton(TranslationEnum.Validate, AddToPatientMonitoringDialog);
            await Assertions.Expect(validateButton).ToBeVisibleAsync();

            ILocator disabledValidateButton = GetButton(TranslationEnum.Validate, AddToPatientMonitoringDialog, true);
            await EnsureVisibleAsync(disabledValidateButton, "Validation button is activated when attempting to add the patient to monitoring");
        }

        private async Task SelectInfectionMonitoringAsync()
        {
            ILocator infectionMonitoringRadio = GetRadio(TranslationEnum.MonitoringOfHai, AddToPatientMonitoringDialog);
            await infectionMonitoringRadio.ClickAsync();
        }

        private async Task VerifyEventSelectDisabled()
        {
            ILocator disabledEventSelect = EventSelect.Locator("byg-select .ng-select.ng-select-disabled");
            await EnsureVisibleAsync(disabledEventSelect, "Event selection is not disabled");
        }
    }
}
