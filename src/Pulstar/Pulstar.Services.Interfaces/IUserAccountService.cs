namespace Pulstar.Services.Interfaces
{
    using Pulstar.Data.Models;

    public interface IUserAccountService
    {
        bool HasEnoughFunds(User user, decimal funds);

        void Withdraw(User user, decimal funds);

        void Deposit(User user, decimal funds);
    }
}
