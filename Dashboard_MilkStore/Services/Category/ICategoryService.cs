using Dashboard_MilkStore.Models.Category;
using Dashboard_MilkStore.Models.Common;

namespace Dashboard_MilkStore.Services.Category
{
    public interface ICategoryService
    {
        Task<ServiceResponse<List<Models.Category.Category>>> GetCategoriesAsync(string? token = null);
        Task<ServiceResponse<Models.Category.Category>> GetCategoryByIdAsync(string id, string? token = null);
        Task<ServiceResponse<Models.Category.Category>> CreateCategoryAsync(CreateCategoryViewModel model, string? token = null);
        Task<ServiceResponse<Models.Category.Category>> UpdateCategoryAsync(string id, Dictionary<string, object> patchValues, string? token = null);
        Task<ServiceResponse<bool>> DeleteCategoryAsync(string id, string? token = null);
    }
}
