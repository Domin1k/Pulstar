namespace Pulstar.Models.Users
{
    using Pulstar.Common.Interfaces;
    using Pulstar.DataModels;

    public class UserListingModel : IMapFrom<User>
    {
        public string Email { get; set; }

        public string UserName { get; set; }
    }
}
