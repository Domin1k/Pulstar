namespace Pulstar.Web.Models.UsersViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class DepositFundsViewModel
    {
        [Range(0, double.MaxValue)]
        public decimal AmountToDeposit { get; set; }
    }
}
