using Dashboard_MilkStore.CoreHelpers;
using Dashboard_MilkStore.Models.Brands;
using Dashboard_MilkStore.Models.Common;
using Dashboard_MilkStore.Models.Product;
using Dashboard_MilkStore.Models.Units;
using System.Net;

namespace Dashboard_MilkStore.Services.Product
{

    public class ProductService : IProductService
    {
        private readonly CallAPI _callAPI;
        private readonly string _baseUrl;

        public ProductService(CallAPI callAPI, IConfiguration configuration)
        {
            _callAPI = callAPI;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001";
        }

        public async Task<ServiceResponse<ProductResponse>> GetProductsAsync(ProductQueryRequest request, string? token = null)
        {
            var url = $"{_baseUrl}/api/Product/get-products";
            var queryString = $"?pageNumber={request.PageNumber}&pageSize={request.PageSize}";

            if (!string.IsNullOrEmpty(request.SearchTerm))
                queryString += $"&searchTerm={Uri.EscapeDataString(request.SearchTerm)}";

            if (!string.IsNullOrEmpty(request.SortBy))
                queryString += $"&sortBy={Uri.EscapeDataString(request.SortBy)}";

            queryString += $"&sortAscending={request.SortAscending}";

            if (!string.IsNullOrEmpty(request.CategoryId))
                queryString += $"&categoryId={Uri.EscapeDataString(request.CategoryId)}";

            if (!string.IsNullOrEmpty(request.TrendId))
                queryString += $"&trendId={Uri.EscapeDataString(request.TrendId)}";

            return await _callAPI.GetAsync<ServiceResponse<ProductResponse>>($"{url}{queryString}", token);
        }

        public async Task<Models.Product.Product?> GetProductByIdAsync(string id)
        {
            try
            {
                var response = await _callAPI.GetAsync<ServiceResponse<Models.Product.Product>>($"{_baseUrl}/api/Product/{id}");
                if (response == null || !response.Success)
                {
                    return null;
                }
                return response.Data;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<ProductPriceDTO>?> GetProductPrice(string productId)
        {
            try
            {
                var response = await _callAPI.GetAsync<ServiceResponse<List<ProductPriceDTO>>>($"{_baseUrl}/api/ProductPrice/{productId}");
                if (response == null || !response.Success)
                {
                    return null;
                }
                return response.Data;
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<ProductStatusDTO>> GetProductStatus()
        {
            try
            {
                var response = await _callAPI.GetAsync<ServiceResponse<List<ProductStatusDTO>>>($"{_baseUrl}/api/ProductStatus");
                if (response == null || !response.Success)
                {
                    return new List<ProductStatusDTO>();
                }
                return response.Data;
            }
            catch
            {
                return new List<ProductStatusDTO>();
            }
        }

        public async Task<List<BrandDTO>> GetBrands()
        {
            try
            {
                var response = await _callAPI.GetAsync<ServiceResponse<List<BrandDTO>>>($"{_baseUrl}/api/Brand");
                if (response == null || !response.Success)
                {
                    return new List<BrandDTO>();
                }
                return response.Data;
            }
            catch
            {
                return new List<BrandDTO>();
            }
        }

        public async Task<List<Dimension>> GetDimensions(string productId)
        {
            try
            {
                var response = await _callAPI.GetAsync<ServiceResponse<List<Dimension>>>($"{_baseUrl}/api/Dimension/{productId}");
                if (response == null || !response.Success)
                {
                    return new List<Dimension>();
                }
                return response.Data;
            }
            catch
            {
                return new List<Dimension>();
            }
        }

        public async Task<List<UnitDTO>> GetUnits()
        {
            try
            {
                var response = await _callAPI.GetAsync<ServiceResponse<List<UnitDTO>>>($"{_baseUrl}/api/Unit");
                if (response == null || !response.Success)
                {
                    return new List<UnitDTO>();
                }
                return response.Data;
            }
            catch
            {
                return new List<UnitDTO>();
            }
        }

        public async Task<ServiceResponse<bool>> UpdateProductAsync(string id, Dictionary<string, object> patchValues)
        {
            try
            {
                // Tạo một đối tượng UpdateProductDTO
                var updateDto = new UpdateProductDTO
                {
                    ProductName = patchValues.ContainsKey("productName") ? patchValues["productName"]?.ToString() : null,
                    Brand = patchValues.ContainsKey("brand1") ? patchValues["brand1"]?.ToString() : null,
                    Sku = patchValues.ContainsKey("sku") ? patchValues["sku"]?.ToString() : null,
                    Bar = patchValues.ContainsKey("bar") ? patchValues["bar"]?.ToString() : null,
                    Description = patchValues.ContainsKey("description") ? patchValues["description"]?.ToString() : null,
                    StatusId = patchValues.ContainsKey("statusId") ? patchValues["statusId"]?.ToString() : null,
                    Unit = patchValues.ContainsKey("unit1") ? patchValues["unit1"]?.ToString() : null
                };

                // Xử lý các trường đặc biệt
                if (patchValues.ContainsKey("stockquantity") && patchValues["stockquantity"] != null)
                {
                    if (int.TryParse(patchValues["stockquantity"].ToString(), out int stockQuantity))
                    {
                        updateDto.Stockquantity = stockQuantity;
                    }
                }

                if (patchValues.ContainsKey("isActive") && patchValues["isActive"] != null)
                {
                    // Xử lý đặc biệt cho giá trị isActive
                    var isActiveValue = patchValues["isActive"];

                    if (isActiveValue is int intValue)
                    {
                        updateDto.IsActive = intValue != 0;
                    }
                    else if (isActiveValue is long longValue)
                    {
                        updateDto.IsActive = longValue != 0;
                    }
                    else if (isActiveValue is string strValue)
                    {
                        if (int.TryParse(strValue, out int parsedInt))
                        {
                            updateDto.IsActive = parsedInt != 0;
                        }
                        else if (bool.TryParse(strValue, out bool parsedBool))
                        {
                            updateDto.IsActive = parsedBool;
                        }
                    }
                    else if (isActiveValue is bool boolValue)
                    {
                        updateDto.IsActive = boolValue;
                    }

                    Console.WriteLine($"isActive value type: {isActiveValue?.GetType().Name}, value: {isActiveValue}, converted: {updateDto.IsActive}");
                }

                // In ra dữ liệu gửi đi để debug
                Console.WriteLine("Sending PUT request to: " + $"{_baseUrl}/api/Product/{id}");
                Console.WriteLine("Data: " + System.Text.Json.JsonSerializer.Serialize(updateDto));

                // In ra giá trị của trường unit1
                Console.WriteLine("unit1 value: " + (patchValues.ContainsKey("unit1") ? patchValues["unit1"]?.ToString() : "null"));
                Console.WriteLine("All keys: " + string.Join(", ", patchValues.Keys));

                // Tạo một mảng các operations cho JsonPatchDocument
                var operations = new List<Dictionary<string, object>>();

                if (updateDto.ProductName != null)
                    operations.Add(new Dictionary<string, object> { { "op", "replace" }, { "path", "/ProductName" }, { "value", updateDto.ProductName } });

                if (updateDto.Brand != null)
                    operations.Add(new Dictionary<string, object> { { "op", "replace" }, { "path", "/Brand" }, { "value", updateDto.Brand } });

                if (updateDto.Sku != null)
                    operations.Add(new Dictionary<string, object> { { "op", "replace" }, { "path", "/Sku" }, { "value", updateDto.Sku } });

                if (updateDto.Bar != null)
                    operations.Add(new Dictionary<string, object> { { "op", "replace" }, { "path", "/Bar" }, { "value", updateDto.Bar } });

                if (updateDto.Stockquantity.HasValue)
                    operations.Add(new Dictionary<string, object> { { "op", "replace" }, { "path", "/Stockquantity" }, { "value", updateDto.Stockquantity.Value } });

                if (updateDto.Unit != null)
                    operations.Add(new Dictionary<string, object> { { "op", "replace" }, { "path", "/Unit" }, { "value", updateDto.Unit } });

                if (updateDto.Description != null)
                    operations.Add(new Dictionary<string, object> { { "op", "replace" }, { "path", "/Description" }, { "value", updateDto.Description } });

                if (updateDto.StatusId != null)
                    operations.Add(new Dictionary<string, object> { { "op", "replace" }, { "path", "/StatusId" }, { "value", updateDto.StatusId } });

                if (updateDto.IsActive.HasValue)
                    operations.Add(new Dictionary<string, object> { { "op", "replace" }, { "path", "/IsActive" }, { "value", updateDto.IsActive.Value } });

                // In ra dữ liệu gửi đi để debug
                Console.WriteLine("Sending PATCH request to: " + $"{_baseUrl}/api/Product/{id}");
                Console.WriteLine("Data: " + System.Text.Json.JsonSerializer.Serialize(operations));

                // Sử dụng PATCH để cập nhật sản phẩm
                var response = await _callAPI.PatchAsync<ServiceResponse<bool>>($"{_baseUrl}/api/Product/{id}", operations);
                return response ?? new ServiceResponse<bool> { Success = false, Message = "Failed to update product" };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Error updating product: {ex.Message}"
                };
            }
        }

        // Image operations
        public async Task<ServiceResponse<ImageDTO>> AddImageAsync(CreateImageDTO createDto, string? token = null)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(createDto.ProductId))
                {
                    return new ServiceResponse<ImageDTO>
                    {
                        Success = false,
                        Message = "ProductId không được để trống",
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }

                if (string.IsNullOrEmpty(createDto.ImageData))
                {
                    return new ServiceResponse<ImageDTO>
                    {
                        Success = false,
                        Message = "ImageData không được để trống",
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }

                // Đảm bảo Order có giá trị
                if (string.IsNullOrEmpty(createDto.Order))
                {
                    createDto.Order = "0";
                }

                // Xử lý dữ liệu base64 nếu cần
                if (createDto.ImageData.Length > 5000000) // ~5MB
                {
                    return new ServiceResponse<ImageDTO>
                    {
                        Success = false,
                        Message = "Kích thước ảnh quá lớn. Vui lòng chọn ảnh nhỏ hơn 5MB.",
                        StatusCode = (int)HttpStatusCode.BadRequest
                    };
                }

                var url = $"{_baseUrl}/api/Image";
                var response = await _callAPI.PostAsync<ServiceResponse<ImageDTO>>(url, createDto, token);

                if (response == null)
                {
                    return new ServiceResponse<ImageDTO>
                    {
                        Success = false,
                        Message = "Failed to add image. No response from server.",
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    };
                }

                // Chuyển đổi dữ liệu từ response.Data sang ImageDTO nếu cần
                if (response.Success && response.Data != null)
                {
                    var imageDTO = new ImageDTO
                    {
                        ImageId = response.Data.ImageId,
                        ImageData = response.Data.ImageData,
                        Order = response.Data.Order
                    };

                    return new ServiceResponse<ImageDTO>
                    {
                        Success = true,
                        Message = response.Message,
                        Data = imageDTO,
                        StatusCode = response.StatusCode
                    };
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddImageAsync: {ex.Message}");
                return new ServiceResponse<ImageDTO>
                {
                    Success = false,
                    Message = $"Error adding image: {ex.Message}",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ServiceResponse<bool>> DeleteImageAsync(string imageId, string? token = null)
        {
            try
            {
                var url = $"{_baseUrl}/api/Image/{imageId}";

                // Create a HttpClient instance
                using (var httpClient = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }

                    var response = await httpClient.DeleteAsync(url);
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync();
                    var result = System.Text.Json.JsonSerializer.Deserialize<ServiceResponse<bool>>(content,
                        new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    return result ?? new ServiceResponse<bool>
                    {
                        Success = true,
                        Message = "Image deleted successfully",
                        StatusCode = (int)HttpStatusCode.OK
                    };
                }
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting image: {ex.Message}",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ServiceResponse<bool>> UpdateImageAsync(string imageId, UpdateImageDTO updateDto, string? token = null)
        {
            try
            {
                var url = $"{_baseUrl}/api/Image/{imageId}";
                var response = await _callAPI.PutAsync<ServiceResponse<bool>>(url, updateDto, token);

                if (response == null)
                {
                    return new ServiceResponse<bool>
                    {
                        Success = false,
                        Message = "Failed to update image. No response from server.",
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    };
                }

                return response;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Error updating image: {ex.Message}",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ServiceResponse<bool>> UpdateImageOrderAsync(string imageId, UpdateImageOrderDTO updateDto, string? token = null)
        {
            try
            {
                // Ghi log để debug
                Console.WriteLine($"UpdateImageOrderAsync: imageId={imageId}, order={updateDto.Order}");

                var url = $"{_baseUrl}/api/Image/{imageId}/order";

                // Sử dụng HttpClient trực tiếp để kiểm soát tốt hơn
                using (var httpClient = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(token))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                    }

                    httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    // Tạo nội dung request - gửi chuỗi order trực tiếp
                    var content = new StringContent(
                        $"\"{updateDto.Order}\"", // Đặt chuỗi trong dấu ngoặc kép để gửi như một JSON string
                        System.Text.Encoding.UTF8,
                        "application/json"
                    );

                    // Ghi log nội dung gửi đi
                    Console.WriteLine($"Sending to {url}: {content.ReadAsStringAsync().Result}");

                    // Gửi request
                    var response = await httpClient.PutAsync(url, content);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    // Ghi log response
                    Console.WriteLine($"Response from {url}: Status={response.StatusCode}, Content={responseContent}");

                    if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(responseContent))
                    {
                        var result = System.Text.Json.JsonSerializer.Deserialize<ServiceResponse<bool>>(responseContent,
                            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                        return result ?? new ServiceResponse<bool>
                        {
                            Success = true,
                            Message = "Image order updated successfully",
                            StatusCode = (int)response.StatusCode
                        };
                    }
                    else
                    {
                        return new ServiceResponse<bool>
                        {
                            Success = false,
                            Message = $"Failed to update image order. Status: {response.StatusCode}, Response: {responseContent}",
                            StatusCode = (int)response.StatusCode
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in UpdateImageOrderAsync: {ex}");
                return new ServiceResponse<bool>
                {
                    Success = false,
                    Message = $"Error updating image order: {ex.Message}",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<ServiceResponse<List<ImageDTO>>> GetImagesByProductIdAsync(string productId, string? token = null)
        {
            try
            {
                var url = $"{_baseUrl}/api/Image/product/{productId}";
                var response = await _callAPI.GetAsync<ServiceResponse<List<ImageDTO>>>(url, token);

                if (response == null)
                {
                    return new ServiceResponse<List<ImageDTO>>
                    {
                        Success = false,
                        Message = "Failed to get images. No response from server.",
                        StatusCode = (int)HttpStatusCode.InternalServerError
                    };
                }

                return response;
            }
            catch (Exception ex)
            {
                return new ServiceResponse<List<ImageDTO>>
                {
                    Success = false,
                    Message = $"Error getting images: {ex.Message}",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }
        }
    }
}