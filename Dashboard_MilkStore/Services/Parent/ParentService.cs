using Dashboard_MilkStore.Models.Common;
using Dashboard_MilkStore.Models.Parent;
using Dashboard_MilkStore.Services.Api;

namespace Dashboard_MilkStore.Services.Parent
{
    public class ParentService : IParentService
    {
        private readonly IApiClient _apiClient;
        private readonly ILogger<ParentService> _logger;

        public ParentService(IApiClient apiClient, ILogger<ParentService> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<ServiceResponse<List<Models.Parent.Parent>>> GetParentsAsync(string? token = null)
        {
            try
            {
                var response = await _apiClient.GetAsync<ServiceResponse<List<Models.Parent.Parent>>>("api/Parent", null, token);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting parents");
                return new ServiceResponse<List<Models.Parent.Parent>>
                {
                    Success = false,
                    Message = $"Lỗi khi lấy danh sách danh mục cha: {ex.Message}",
                    Data = new List<Models.Parent.Parent>()
                };
            }
        }

        public async Task<ServiceResponse<Models.Parent.Parent>> GetParentByIdAsync(string id, string? token = null)
        {
            try
            {
                var response = await _apiClient.GetAsync<ServiceResponse<Models.Parent.Parent>>($"api/Parent/{id}", null, token);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting parent with ID: {Id}", id);
                return new ServiceResponse<Models.Parent.Parent>
                {
                    Success = false,
                    Message = $"Lỗi khi lấy thông tin danh mục cha: {ex.Message}",
                    Data = null
                };
            }
        }
    }
}
