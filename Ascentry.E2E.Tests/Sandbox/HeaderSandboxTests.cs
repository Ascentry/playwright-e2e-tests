using Ascentry.E2E.Contracts.Enums;
using Ascentry.E2E.Tests.Attributes;
using Xunit;

namespace Ascentry.E2E.Tests.Sandbox
{
    [Products(ProductEnum.InfectionTracker)]
    public class HeaderSandboxTests : AbstractTestBase
    {
        [Fact(DisplayName = "Log out user")]
        public async Task Should_Logout_User()
        {
            await Header.LogoutUserAsync();
        }
    }
}
