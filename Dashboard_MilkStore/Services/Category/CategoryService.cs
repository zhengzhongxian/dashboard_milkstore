using Dashboard_MilkStore.CoreHelpers;
using Dashboard_MilkStore.Models.Category;
using Dashboard_MilkStore.Models.Common;
using Dashboard_MilkStore.Services.Api;
using System.Text.Json;

namespace Dashboard_MilkStore.Services.Category
{
    public class CategoryService : ICategoryService
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(IApiClient apiClient, ILogger<CategoryService> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<ServiceResponse<List<Models.Category.Category>>> GetCategoriesAsync(string? token = null)
        {
            try
            {
                var response = await _apiClient.GetAsync<ServiceResponse<List<Models.Category.Category>>>("api/Category", null, token);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories");
                return new ServiceResponse<List<Models.Category.Category>>
                {
                    Success = false,
                    Message = $"Lỗi khi lấy danh sách danh mục: {ex.Message}",
                    Data = new List<Models.Category.Category>()
                };
            }
        }

        public async Task<ServiceResponse<Models.Category.Category>> GetCategoryByIdAsync(string id, string? token = null)
        {
            try
            {
                var response = await _apiClient.GetAsync<ServiceResponse<Models.Category.Category>>($"api/Category/{id}", null, token);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category with ID: {Id}", id);
                return new ServiceResponse<Models.Category.Category>
                {
                    Success = false,
                    Message = $"Lỗi khi lấy thông tin danh mục: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ServiceResponse<Models.Category.Category>> CreateCategoryAsync(CreateCategoryViewModel model, string? token = null)
        {
            try
            {
                var response = await _apiClient.PostAsync<ServiceResponse<Models.Category.Category>>("api/Category", model, token);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return new ServiceResponse<Models.Category.Category>
                {
                    Success = false,
                    Message = $"Lỗi khi tạo danh mục: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ServiceResponse<Models.Category.Category>> UpdateCategoryAsync(string id, Dictionary<string, object> patchValues, string? token = null)
        {
            try
            {
                // Tạo JsonPatchDocument từ Dictionary
                var patchDoc = new List<Dictionary<string, object>>();
                foreach (var kvp in patchValues)
                {
                    patchDoc.Add(new Dictionary<string, object>
                    {
                        { "op", "replace" },
                        { "path", $"/{kvp.Key}" },
                        { "value", kvp.Value }
                    });
                }

                var response = await _apiClient.PatchAsync<ServiceResponse<Models.Category.Category>>($"api/Category/{id}", patchDoc, token);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category with ID: {Id}", id);
                return new ServiceResponse<Models.Category.Category>
                {
                    Success = false,
                    Message = $"Lỗi khi cập nhật danh mục: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteCategoryAsync(string id, string? token = null)
        {
            try
            {
                var response = await _apiClient.DeleteAsync<ServiceResponse<bool>>($"api/Category/{id}", token);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category with ID: {Id}", id);
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Lỗi khi xóa danh mục: {ex.Message}",
                    Data = false
                };
            }
        }
    }
}
