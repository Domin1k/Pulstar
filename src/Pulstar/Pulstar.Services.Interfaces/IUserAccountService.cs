namespace Pulstar.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface IUserAccountService
    {
        bool HasEnoughFunds(decimal funds);

        void Withdraw(decimal funds);

        void Deposit(decimal funds);
    }
}
