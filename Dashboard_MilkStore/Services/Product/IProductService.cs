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
        Task<ServiceResponse<Models.Product.Product>> CreateProductAsync(CreateProductDTO createDto, string? token = null);

        // Image operations
        Task<ServiceResponse<ImageDTO>> AddImageAsync(CreateImageDTO createDto, string? token = null);
        Task<ServiceResponse<bool>> DeleteImageAsync(string imageId, string? token = null);
        Task<ServiceResponse<bool>> UpdateImageAsync(string imageId, UpdateImageDTO updateDto, string? token = null);
        Task<ServiceResponse<bool>> UpdateImageOrderAsync(string imageId, UpdateImageOrderDTO updateDto, string? token = null);

        // Dimension operations
        Task<ServiceResponse<Dimension>> AddDimensionAsync(CreateDimensionDTO createDto, string? token = null);
        Task<ServiceResponse<bool>> UpdateDimensionAsync(string dimensionId, Dictionary<string, object> patchValues, string? token = null);
        Task<ServiceResponse<bool>> DeleteDimensionAsync(string dimensionId, string? token = null);

        // Product Category operations
        Task<ServiceResponse<List<ProductCategoryViewModel>>> GetProductCategoriesAsync(string productId, string? token = null);
        Task<ServiceResponse<bool>> ProcessProductCategoryChangesAsync(string productId, ProductCategoryChangesViewModel changes, string? token = null);
        Task<ServiceResponse<ProductCategoryViewModel>> AddProductCategoryAsync(CreateProductCategoryViewModel createViewModel, string? token = null);
        Task<ServiceResponse<bool>> DeleteProductCategoryAsync(string productId, string categoryId, string? token = null);
        Task<ServiceResponse<List<ImageDTO>>> GetImagesByProductIdAsync(string productId, string? token = null);

        // ProductPrice operations
        Task<ServiceResponse<ProductPriceDTO>> AddProductPriceAsync(CreateProductPriceDTONew createDto, string? token = null);
        Task<ServiceResponse<bool>> UpdateProductPriceAsync(string priceId, Dictionary<string, object> patchValues, string? token = null);
        Task<ServiceResponse<bool>> DeleteProductPriceAsync(string priceId, string? token = null);
        Task<ServiceResponse<bool>> SetDefaultPriceAsync(string priceId, bool isDefault, string? token = null);
        Task<ServiceResponse<bool>> SetPriceStatusAsync(string priceId, bool isActive, string? token = null);
        Task<ProductPriceDTO?> GetProductPriceByIdAsync(string priceId, string? token = null);

        // Product operations
        Task<ServiceResponse<bool>> DeleteProductAsync(string productId, string? token = null);
    }
}