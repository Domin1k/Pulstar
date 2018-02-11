namespace Pulstar.Services
{
    using System;
    using Pulstar.Common.Constants;
    using Pulstar.Data;
    using Pulstar.Data.Models;
    using Pulstar.Services.Interfaces;

    public class UserAccountService : IUserAccountService
    {
        private readonly PulstarDbContext _pulstarDb;

        public UserAccountService(PulstarDbContext pulstarDb)
        {
            _pulstarDb = pulstarDb;
        }

        public void Deposit(User user, decimal funds)
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

            _pulstarDb.Users.Update(user);
        }

        public bool HasEnoughFunds(User user, decimal funds) => (user.AccountBalance - funds) > 0m;

        public void Withdraw(User user, decimal funds)
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

            _pulstarDb.Users.Update(user);
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
