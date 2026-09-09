using Ascentry.E2E.Contracts.Enums;
using Ascentry.E2E.Contracts.ExpertRules;
using Ascentry.E2E.Navigations.Menus;
using Ascentry.E2E.Testing;
using Ascentry.E2E.Tests.Attributes;
using Xunit;

namespace Ascentry.E2E.Tests.Sandbox
{
    // Faire en sorte d'utiliser la même instance Playwright et le même context et même connexion pour les 100 scénarios de tests
    // Utilisation du spinner pour vérifier l'état de la page
    [Products(ProductEnum.InfectionTracker)]
    public class ExpertRuleSandboxTests : AscentryTestBase, IClassFixture<AscentryFixture>
    {
        public ExpertRuleSandboxTests(AscentryFixture fixture) : base(fixture) { }

        [Fact(DisplayName = "Execute an expert rule in test mode - Success - one row with one condition with one instruction")]
        public async Task Should_Execute_Test_Mode_One_Row_One_Condition_One_Instruction()
        {
            await NavigationMenu.GoToAsync(InfectionTrackerMenu.Settings.Expertise.ExpertRules);

            var expertRule = await ExpertRuleListPage.OpenAsync("101309-Générer un évènement");
            await expertRule.VerifyEditModeAsync();

            ExpertRuleResult result = await expertRule.ExecuteTestModeAsync(ExpertRuleTestModeContextTypeEnum.PatientPid, "P11161287623");
            Assert.Single(result.RowResults);
            Assert.True(result.RowResults[0].IsSuccess);
            Assert.True(result.RowResults[0].RowConditionResults[0].IsSuccess);
            Assert.True(result.RowResults[0].RowConditionResults[0].RowInstructionResults[0].IsSuccess);
        }

        [Fact(DisplayName = "Execute an expert rule in test mode - Success - multiple rows with at leat one condition block with multiple instructions")]
        public async Task Should_Execute_Test_Mode_Multiple_Rows_Multiple_Conditions_Multiple_Instructions()
        {
            await NavigationMenu.GoToAsync(InfectionTrackerMenu.Settings.Expertise.ExpertRules);

            var expertRule = await ExpertRuleListPage.OpenAsync("Doublon SPECIMEN EVT GEST");
            await expertRule.VerifyEditModeAsync();

            ExpertRuleResult result = await expertRule.ExecuteTestModeAsync(ExpertRuleTestModeContextTypeEnum.SpecimenNumber, "100016999190");
            Assert.Equal(2, result.RowResults.Count);
            Assert.True(result.RowResults[0].IsSuccess);
            Assert.True(result.RowResults[0].RowConditionResults[0].IsSuccess);
            Assert.True(result.RowResults[0].RowConditionResults[0].RowInstructionResults[0].IsSuccess);
            Assert.False(result.RowResults[0].RowConditionResults[0].RowInstructionResults[1].IsSuccess);
            Assert.False(result.RowResults[0].RowConditionResults[0].RowInstructionResults[2].IsSuccess);

            Assert.True(result.RowResults[0].RowConditionResults[1].IsSuccess);
            Assert.True(result.RowResults[0].RowConditionResults[1].IsSuccess);
            Assert.True(result.RowResults[0].RowConditionResults[1].RowInstructionResults[0].IsSuccess);
            Assert.False(result.RowResults[1].IsSuccess);
        }
    }
}