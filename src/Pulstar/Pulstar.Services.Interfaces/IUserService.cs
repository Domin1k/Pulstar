namespace Pulstar.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Pulstar.Common.Constants;
    using Pulstar.Models.Products;
    using Pulstar.Models.Users;

    public interface IUserService
    {
        Task<IEnumerable<UserListingModel>> All(int takeCount = AppConstants.DefaultTakeCount);
        
        Task AssignToRole(string userName, string roleName);

        Task AddPaymentMethod(string userName, string creditCardNumber, string cvv, DateTime expirationDate);

        Task<IEnumerable<UserPaymentMethodsModel>> PaymentMethods(string userName);

        Task DepositFunds(string userName, decimal amountToDeposit, IUserAccountService userAccountService);
    }
}
