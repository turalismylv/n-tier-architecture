using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Services.Abstract;
using Web.ViewModels.Product;
using Web.ViewModels.Product.ProductPhoto;

namespace Web.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {

            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _productService.GetAllAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await _productService.GetCreateModelAsync();
            return View(model);
        }

        [HttpPost]

        public async Task<IActionResult> Create(ProductCreateVM model)
        {


            var isSucceeded = await _productService.CreateAsync(model);
            if (isSucceeded) return RedirectToAction(nameof(Index));
            model = await _productService.GetCreateModelAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var model = await _productService.GetUpdateModelAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, ProductUpdateVM model)
        {
            if (id != model.Id) return NotFound();

            var isSucceeded = await _productService.UpdateAsync(model);
            if (isSucceeded) return RedirectToAction(nameof(Index));

            model = await _productService.GetUpdateModelAsync(id);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var isSucceded = await _productService.DeleteAsync(id);
            if (isSucceded)
            {
                return RedirectToAction(nameof(Index));
            }
            return NotFound();


        }

        [HttpPost]
        public async Task<IActionResult> DeletePhoto(int id, int productId)
        {

            var isSucceded = await _productService.DeletePhotoAsync(id);
            if (isSucceded) return RedirectToAction("update", "product", new { id = productId });

            return NotFound();


        }

        [HttpGet]
        public async Task<IActionResult> UpdatePhoto(int id)
        {
            var model = await _productService.GetPhotoUpdateModelAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePhoto(int id, ProductPhotoUpdateVM model)
        {
            if (id != model.Id) return NotFound();

            var isSucceeded = await _productService.UpdatePhotoAsync(model);
            if (isSucceeded) return RedirectToAction("update", "product", new { id = model.ProductId });
            
            return View(model);
        }

    }
}
