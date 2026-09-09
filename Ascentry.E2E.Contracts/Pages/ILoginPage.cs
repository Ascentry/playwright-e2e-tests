namespace Ascentry.E2E.Contracts.Interfaces
{
    public interface ILoginPage
    {
        Task LoginUserAsync(string username, string password);
    }
}
