using Web.ViewModels.Product;

namespace Web.Services.Abstract
{
    public interface IProductService
    {
        Task<ProductIndexVM> GetAllAsync();
        Task<ProductCreateVM> GetCreateModelAsync();
        Task<bool> CreateAsync(ProductCreateVM model);

        Task<ProductUpdateVM> GetUpdateModelAsync(int id);

        Task<bool> UpdateAsync(ProductUpdateVM model);

        Task<bool> DeleteAsync(int id);

    }
}
