namespace Pulstar.Models.Category
{
    using Pulstar.Common.Enums;
    using Pulstar.Common.Interfaces;
    using Pulstar.Data.Models;

    public class CategoryDetailsModel : IMapFrom<Category>
    {
        public string Name { get; set; }

        public CategoryType Type { get; set; }
    }
}
