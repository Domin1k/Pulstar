namespace Pulstar.Web.Infrastructure.Extensions
{
    using System.Security.Claims;
    using Pulstar.Common.Constants;

    public static class UserExtensions
    {
        public static bool HasPermissions(this ClaimsPrincipal principal)
            => principal.IsInRole(AppConstants.Administrator) || principal.IsInRole(AppConstants.Manager);
    }
}
