namespace Pulstar.Services
{
    using System;
    using Pulstar.Data.Models;
    using Pulstar.Services.Interfaces;

    public class UserAccountService : IUserAccountService
    {
        private User _user;

        public UserAccountService(User user)
        {
            _user = user ?? throw new ArgumentNullException($"{nameof(user)} cannot be null.");
        }

        public void Deposit(decimal funds)
        {
            decimal balanceBefore = balanceBefore = _user.AccountBalance;
            try
            {
                if (funds <= 0)
                {
                    throw new InvalidOperationException($"Funds must be equal or greater than zero.");
                }

                _user.AccountBalance += funds;
            }
            catch (Exception)
            {
                // TODO log error.
                DoRollback(balanceBefore);
                throw;
            }
        }

        public bool HasEnoughFunds(decimal funds)
        {
            return (_user.AccountBalance - funds) > 0m;
        }

        public void Withdraw(decimal funds)
        {
            decimal balanceBefore = _user.AccountBalance;
            try
            {
                if (funds <= 0)
                {
                    throw new InvalidOperationException($"Funds must be equal or greater than zero.");
                }

                _user.AccountBalance -= funds;
            }
            catch (Exception)
            {
                // TODO log error.
                DoRollback(balanceBefore);
                throw;
            }
        }

        private void DoRollback(decimal amountBefore)
        {
            var currentBalance = _user.AccountBalance;
            if (amountBefore != currentBalance)
            {
                _user.AccountBalance = currentBalance;
            }
        }
    }
}
