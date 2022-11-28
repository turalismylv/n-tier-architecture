using Web.ViewModels.Expert;

namespace Web.Services.Abstract
{
    public interface IExpertService
    {
        Task<ExpertIndexVM> GetAllAsync();

        Task<bool> CreateAsync(ExpertCreateVM model);

        Task<ExpertUpdateVM> GetUpdateModelAsync(int id);
        Task<bool> UpdateAsync(ExpertUpdateVM model);

        Task<bool> DeleteAsync(int id);
    }
}
