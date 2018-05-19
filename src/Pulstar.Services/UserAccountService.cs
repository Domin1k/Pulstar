namespace Pulstar.Services
{
    using System;
    using System.Threading.Tasks;
    using Pulstar.Common.Constants;
    using Pulstar.Data.Interfaces;
    using Pulstar.DataModels;
    using Pulstar.Services.Interfaces;

    public class UserAccountService : IUserAccountService
    {
        private readonly IRepository<User> _usersRepository;

        public UserAccountService(IRepository<User> usersRepostory)
        {
            _usersRepository = usersRepostory;
        }

        public async Task DepositAsync(User user, decimal funds)
        {
            decimal balanceBefore = balanceBefore = user.AccountBalance;
            try
            {
                if (funds <= 0)
                {
                    throw new InvalidOperationException(ServiceErrorsConstants.InvalidFunds);
                }

                user.AccountBalance += funds;
            }
            catch (Exception)
            {
                // TODO log error.
                DoRollback(user, balanceBefore);
                throw;
            }

            _usersRepository.Update(user);

            await _usersRepository.SaveChangesAsync();
        }

        public bool HasEnoughFunds(User user, decimal funds) => (user.AccountBalance - funds) > 0m;

        public async Task WithdrawAsync(User user, decimal funds)
        {
            decimal balanceBefore = user.AccountBalance;
            try
            {
                if (funds <= 0)
                {
                    throw new InvalidOperationException(ServiceErrorsConstants.InvalidFunds);
                }

                user.AccountBalance -= funds;
            }
            catch (Exception)
            {
                // TODO log error.
                DoRollback(user, balanceBefore);
                throw;
            }

            _usersRepository.Update(user);
            await _usersRepository.SaveChangesAsync();
        }

        private void DoRollback(User user, decimal amountBefore)
        {
            var currentBalance = user.AccountBalance;
            if (amountBefore != currentBalance)
            {
                user.AccountBalance = currentBalance;
            }
        }
    }
}
