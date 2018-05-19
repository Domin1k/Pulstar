namespace Pulstar.Services.Models.Category
{
    using Pulstar.Common.Interfaces;
    using Pulstar.DataModels;

    public class CategoryListingServiceModel : IMapFrom<Category>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
