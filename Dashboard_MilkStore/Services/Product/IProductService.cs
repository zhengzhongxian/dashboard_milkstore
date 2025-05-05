using Dashboard_MilkStore.Models.Brands;
using Dashboard_MilkStore.Models.Common;
using Dashboard_MilkStore.Models.Product;
using Dashboard_MilkStore.Models.Units;

namespace Dashboard_MilkStore.Services.Product
{
    public interface IProductService
    {
        Task<ServiceResponse<ProductResponse>> GetProductsAsync(ProductQueryRequest request, string? token = null);
        Task<Models.Product.Product?> GetProductByIdAsync(string id);
        Task<List<ProductPriceDTO>?> GetProductPrice(string productId);
        Task<List<ProductStatusDTO>> GetProductStatus();
        Task<List<BrandDTO>> GetBrands();
        Task<List<Dimension>> GetDimensions(string productId);
        Task<List<UnitDTO>> GetUnits();
        Task<ServiceResponse<bool>> UpdateProductAsync(string id, Dictionary<string, object> patchValues);

        // Image operations
        Task<ServiceResponse<ImageDTO>> AddImageAsync(CreateImageDTO createDto, string? token = null);
        Task<ServiceResponse<bool>> DeleteImageAsync(string imageId, string? token = null);
        Task<ServiceResponse<bool>> UpdateImageAsync(string imageId, UpdateImageDTO updateDto, string? token = null);
        Task<ServiceResponse<bool>> UpdateImageOrderAsync(string imageId, UpdateImageOrderDTO updateDto, string? token = null);
        Task<ServiceResponse<List<ImageDTO>>> GetImagesByProductIdAsync(string productId, string? token = null);
    }
}