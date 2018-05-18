namespace Pulstar.Services.Interfaces
{
    using System.Threading.Tasks;
    using Pulstar.DataModels;

    public interface IUserAccountService
    {
        bool HasEnoughFunds(User user, decimal funds);

        Task Withdraw(User user, decimal funds);

        Task Deposit(User user, decimal funds);
    }
}
