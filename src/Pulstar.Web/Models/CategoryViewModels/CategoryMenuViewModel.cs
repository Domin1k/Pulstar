namespace Pulstar.Web.Models.CategoryViewModels
{
    using System.Collections.Generic;

    public class CategoryMenuViewModel
    {
        public IEnumerable<string> Games { get; set; }

        public IEnumerable<string> Consoles { get; set; }

        public IEnumerable<string> Accessories { get; set; }
    }
}
