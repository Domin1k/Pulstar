namespace Pulstar.Data.Extensions
{
    using Pulstar.DataModels;

    public static class DataModelExtensions
    {
        public static void SetUniquePurchaseCode(this Purchase purchase)
        {
            var uniqueCode = purchase.Id + purchase.Date.Ticks;
            purchase.UniqueCode = uniqueCode;
        }

        public static long GetUniquePurchaseCode(this Purchase purchase)
        {
            return purchase.Id + purchase.Date.Ticks;
        }
    }
}
