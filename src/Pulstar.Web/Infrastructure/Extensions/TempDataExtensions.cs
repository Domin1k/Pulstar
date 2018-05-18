namespace Pulstar.Web.Infrastructure.Extensions
{
    using Microsoft.AspNetCore.Mvc.ViewFeatures;

    public static class TempDataExtensions
    {
        public const string TempDataErrorKey = "TempDataError";
        public const string TempDataSuccess = "TempDataSuccess";

        public static void AddErrorMessage(this ITempDataDictionary tempData, string errorMessage)
        {
            errorMessage = errorMessage ?? "Error occurred.Please try again.";
            tempData[TempDataErrorKey] = errorMessage;
        }

        public static void AddSuccessMessage(this ITempDataDictionary tempData, string successMessage)
        {
            tempData[TempDataSuccess] = successMessage;
        }
    }
}
