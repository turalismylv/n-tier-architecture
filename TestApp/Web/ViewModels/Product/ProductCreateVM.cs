using Core.Constants;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.ViewModels.Product
{
    public class ProductCreateVM
    {
        public string Title { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }

        public string Weight { get; set; }
        

        public int CategoryId { get; set; }
        public List<SelectListItem>? Categories { get; set; }
        public ProductStatus Status { get; set; }

        public IFormFile MainPhoto { get; set; }

        public List<IFormFile>? Photos { get; set; }
    }

}
