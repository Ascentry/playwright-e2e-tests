using Ascentry.E2E.Contracts.Enums;
using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace Ascentry.E2E.Pages.InfectionTracker.Epidemiology.Infections
{
    internal class EntityContextQuestionnairePageBase : PageBase
    {
        public EntityContextQuestionnairePageBase(IPage page) : base(page) { }

        protected async Task<QuestionnaireStatusEnum> GetQuestionnaireStatusAsync(ILocator parent)
        {
            ILocator emptyQuestionnaireIcon = GetIconBySvgGroupId("assignment", parent);

            if (await emptyQuestionnaireIcon.CountAsync() > 0)
            {
                return QuestionnaireStatusEnum.Unassociated;
            }

            ILocator filledQuestionnaireIcon = GetIconBySvgGroupId("validation_questionnaire", parent);

            if (await filledQuestionnaireIcon.CountAsync() > 0)
            {
                return QuestionnaireStatusEnum.Filled;
            }

            ILocator uncompleteQuestionnaireIcon = GetIconBySvgGroupId("questionnaire", parent);

            if (await filledQuestionnaireIcon.CountAsync() > 0)
            {
                return QuestionnaireStatusEnum.Uncomplete;
            }

            throw new NotSupportedException("Not supported value for questionnaire status");
        }
    }
}
