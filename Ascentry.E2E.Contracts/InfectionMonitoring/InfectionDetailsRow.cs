using Ascentry.E2E.Contracts.Enums;

namespace Ascentry.E2E.Contracts.InfectionMonitoring
{
    public class InfectionDetailsRow
    {
        public string PatientLastName { get; set; }
        public string PatientFirstName { get; set; }
        public string SampleType { get; set; }
        public string SpecimenCollectionDate { get; set; }
        public string CareUnit { get; set; }
        public string InfectionType { get; set; }
        public string InfectionStatus { get; set; }
        public string InfectionQualification { get; set; }
        public bool HasEmptyComments { get; set; }
        public QuestionnaireStatusEnum QuestionnaireStatus { get; set; }
    }
}
