namespace Pulstar.Services.Interfaces
{
    using System.Threading.Tasks;
    using Pulstar.DataModels;

    public interface IUserAccountService
    {
        bool HasEnoughFunds(User user, decimal funds);

        Task WithdrawAsync(User user, decimal funds);

        Task DepositAsync(User user, decimal funds);
    }
}
