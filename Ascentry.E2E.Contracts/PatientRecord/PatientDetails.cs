namespace Ascentry.E2E.Contracts.PatientRecord
{
    public class PatientDetails
    {
        public string PatientLastName { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientBirthDate { get; set; }
        public List<string> Pids { get; set; }
    }
}
