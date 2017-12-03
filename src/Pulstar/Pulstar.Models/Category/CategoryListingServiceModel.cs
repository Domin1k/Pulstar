namespace Pulstar.Models.Category
{
    using Pulstar.Common.Interfaces;
    using Pulstar.Data.Models;

    public class CategoryListingServiceModel : IMapFrom<Category>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
