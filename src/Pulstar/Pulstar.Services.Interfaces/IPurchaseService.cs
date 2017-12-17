namespace Pulstar.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Pulstar.Models.Purchase;

    public interface IPurchaseService
    {
        Task AddPurchase(IEnumerable<PurchaseProduct> purchaseProducts, string userName);

        Task<IEnumerable<PurchaseListingModel>> Products(string userName);
    }
}
