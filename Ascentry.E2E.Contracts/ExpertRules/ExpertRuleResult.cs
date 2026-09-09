namespace Ascentry.E2E.Contracts.ExpertRules
{
    public class ExpertRuleResult
    {
        public List<ExpertRuleRowResult> RowResults { get; set; }
    }

    public class ExpertRuleRowResult
    {
        public bool IsSuccess { get; set; }
        public List<ExpertRuleRowConditionResult> RowConditionResults { get; set; }
    }

    public class ExpertRuleRowConditionResult
    {
        public bool IsSuccess { get; set; }
        public List<ExpertRuleRowInstructionResult> RowInstructionResults { get; set; }
    }


    public class ExpertRuleRowInstructionResult
    {
        public string InstructionLabel { get; set; }
        public bool IsSuccess { get; set; }
    }
}
