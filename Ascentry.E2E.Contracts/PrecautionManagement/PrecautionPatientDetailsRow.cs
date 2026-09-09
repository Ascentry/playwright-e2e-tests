using Ascentry.E2E.Contracts.Enums;

namespace Ascentry.E2E.Contracts.PrecautionManagement
{
    public class PatientDetailsRow
    {
        public string PatientLastName { get; set; }
        public string PatientFirstName { get; set; }
        public List<string> Pids { get; set; }

        public string BirthDate { get; set; }
        public string AdmissionDate { get; set; }
        public string ReleaseDate { get; set; }
        public string LastCareUnit { get; set; }
        public bool HasInfections { get; set; }
        public bool HasContacts { get; set; }
        public List<PatientPrecautionDetailsRow> Precautions { get; set; }

    }

    public class PatientPrecautionDetailsRow
    {
        public string EventName { get; set; }
        public string SampleType { get; set; }
        public string SpecimenCollectionDate { get; set; }
        public string SpecimenNumber { get; set; }
        public string EventLocation { get; set; }
        public string PrecautionType { get; set; }
        public string PrecautionStartDate { get; set; }
        public string PrecautionEndDate { get; set; }
        public string PrecautionDuration { get; set; }
        public QuestionnaireStatusEnum QuestionnaireStatus { get; set; }

    }
}