using Ascentry.E2E.Contracts.Common;

namespace Ascentry.E2E.Contracts.Layout
{
    public interface INavigationMenu
    {
        Task GoToAsync(IEnumerable<MenuNode> nodes);
    }
}
