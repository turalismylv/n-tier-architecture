using Web.Areas.Admin.ViewModels.Category;

namespace Web.Areas.Admin.Services.Abstract
{
    public interface ICategoryService
    {

        Task<CategoryIndexVM> GetAllAsync();
    }
}
