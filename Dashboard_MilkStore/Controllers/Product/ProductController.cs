using Dashboard_MilkStore.Models.Product;
using Dashboard_MilkStore.Services.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dashboard_MilkStore.Controllers.Product
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductController(IProductService productService, IHttpContextAccessor httpContextAccessor)
        {
            _productService = productService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index(string? searchTerm = null, string? sortBy = null, bool sortAscending = true, int pageNumber = 1, int pageSize = 10)
        {
            var request = new ProductQueryRequest
            {
                SearchTerm = searchTerm,
                SortBy = sortBy,
                SortAscending = sortAscending,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
            var response = await _productService.GetProductsAsync(request, token);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message;
                return View(new ProductResponse());
            }

            return View(response.Data);
        }

        public async Task<IActionResult> QuickView(string id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    return NotFound();
                }
                return PartialView("QuickView", product);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var viewModel = new ProductEditViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Sku = product.SKU,
                Bar = product.Bar,
                Brand1 = product.Brand1,
                StockQuantity = product.StockQuantity,
                Unit1 = product.UnitId,
                Description = product.Description,
                StatusId = product.StatusId,
                IsActive = product.IsActive ?? true,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                Dimensions = await _productService.GetDimensions(product.ProductId) ?? new List<Dimension>(),
                Images = product.ImageDTOs.Select(i => new ImageDTO
                {
                    ImageId = i.ImageId,
                    ImageData = i.ImageData,
                    Order = i.Order
                }).ToList(),
                Prices = await _productService.GetProductPrice(product.ProductId) ?? new List<ProductPriceDTO>(),
                Statuses = (await _productService.GetProductStatus())
                    .Select(s => new SelectListItem
                    {
                        Value = s.StatusId,
                        Text = s.Name
                    }).ToList(),
                Brands = (await _productService.GetBrands())
                    .Select(b => new SelectListItem
                    {
                        Value = b.Brand1,
                        Text = b.BrandName,
                    }).ToList(),
                Units = (await _productService.GetUnits())
                    .Select(u => new SelectListItem
                    {
                        Value = u.Unit1,
                        Text = u.UnitName ?? u.Unit1,
                    }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] Dictionary<string, object> patchValues)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "ID sản phẩm không hợp lệ" });
            }

            try
            {
                var response = await _productService.UpdateProductAsync(id, patchValues);

                // Luôn trả về thành công nếu không có lỗi
                return Json(new { success = true, message = "Cập nhật sản phẩm thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi cập nhật sản phẩm: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveImageChanges(string productId,
            [FromBody] ProductImageChangesViewModel changes)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return Json(new { success = false, message = "ID sản phẩm không hợp lệ" });
            }

            try
            {
                var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
                var successCount = 0;
                var errorMessages = new List<string>();

                // 1. Process deleted images
                if (changes.DeletedImageIds != null && changes.DeletedImageIds.Any())
                {
                    foreach (var imageId in changes.DeletedImageIds)
                    {
                        var deleteResponse = await _productService.DeleteImageAsync(imageId, token);
                        if (deleteResponse.Success)
                        {
                            successCount++;
                        }
                        else
                        {
                            errorMessages.Add($"Lỗi khi xóa ảnh {imageId}: {deleteResponse.Message}");
                        }
                    }
                }

                // 2. Process new images
                if (changes.NewImages != null && changes.NewImages.Any())
                {
                    foreach (var newImage in changes.NewImages)
                    {
                        try
                        {
                            // Kiểm tra và xử lý dữ liệu ảnh
                            string imageData = newImage.ImageData;

                            // Nếu là URL, bỏ qua vì không cần xử lý
                            if (imageData.StartsWith("http"))
                            {
                                errorMessages.Add($"Không thể thêm ảnh từ URL. Vui lòng tải ảnh từ máy tính.");
                                continue;
                            }

                            // Nếu là base64, đảm bảo định dạng đúng
                            if (imageData.Contains("base64,"))
                            {
                                imageData = imageData.Substring(imageData.IndexOf("base64,") + 7);
                            }

                            // Kiểm tra kích thước ảnh
                            if (imageData.Length > 5000000) // ~5MB
                            {
                                errorMessages.Add($"Ảnh có kích thước quá lớn. Vui lòng chọn ảnh nhỏ hơn 5MB.");
                                continue;
                            }

                            var createDto = new CreateImageDTO
                            {
                                ProductId = productId,
                                ImageData = imageData,
                                Order = string.IsNullOrEmpty(newImage.Order) ? "0" : newImage.Order
                            };

                            Console.WriteLine($"Sending image data: ProductId={createDto.ProductId}, Order={createDto.Order}, ImageData length={createDto.ImageData.Length}");

                            var addResponse = await _productService.AddImageAsync(createDto, token);
                            if (addResponse.Success)
                            {
                                successCount++;
                                Console.WriteLine($"Successfully added image: {addResponse.Message}");
                            }
                            else
                            {
                                errorMessages.Add($"Lỗi khi thêm ảnh mới: {addResponse.Message}");
                                Console.WriteLine($"Failed to add image: {addResponse.Message}");
                            }
                        }
                        catch (Exception ex)
                        {
                            errorMessages.Add($"Lỗi xử lý ảnh: {ex.Message}");
                            Console.WriteLine($"Exception when processing image: {ex.Message}");
                        }
                    }
                }

                // 3. Process updated orders
                if (changes.UpdatedOrders != null && changes.UpdatedOrders.Any())
                {
                    foreach (var update in changes.UpdatedOrders)
                    {
                        var updateOrderDto = new UpdateImageOrderDTO
                        {
                            Order = update.Order
                        };

                        var updateResponse = await _productService.UpdateImageOrderAsync(update.ImageId, updateOrderDto, token);
                        if (updateResponse.Success)
                        {
                            successCount++;
                        }
                        else
                        {
                            errorMessages.Add($"Lỗi khi cập nhật thứ tự ảnh {update.ImageId}: {updateResponse.Message}");
                        }
                    }
                }

                // Return result
                if (errorMessages.Any())
                {
                    return Json(new {
                        success = successCount > 0,
                        message = $"Đã xử lý {successCount} thao tác thành công. Có lỗi: {string.Join(", ", errorMessages)}"
                    });
                }
                else
                {
                    return Json(new {
                        success = true,
                        message = $"Đã cập nhật {successCount} thay đổi hình ảnh thành công"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi cập nhật hình ảnh: {ex.Message}" });
            }
        }

        // Cập nhật thứ tự ảnh
        [HttpPost]
        public async Task<IActionResult> UpdateImageOrder([FromBody] UpdateOrderRequest request)
        {
            try
            {
                // Thêm log để debug
                System.Diagnostics.Debug.WriteLine($"UpdateImageOrder: imageId={request.ImageId}, order={request.Order}");

                if (string.IsNullOrEmpty(request.ImageId))
                {
                    return Json(new { success = false, message = "ImageId không được để trống" });
                }

                var token = HttpContext.Session.GetString("Token");
                var updateDto = new UpdateImageOrderDTO { Order = request.Order };
                var response = await _productService.UpdateImageOrderAsync(request.ImageId, updateDto, token);

                return Json(new { success = response.Success, message = response.Message });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in UpdateImageOrder: {ex}");
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }

        // Model cho request cập nhật thứ tự ảnh đơn lẻ
        public class UpdateOrderRequest
        {
            public string ImageId { get; set; }
            public string Order { get; set; }
        }

        // Cập nhật nhiều thứ tự ảnh cùng lúc
        [HttpPost]
        public async Task<IActionResult> UpdateMultipleImageOrders([FromBody] BatchImageOrderUpdate model)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (model == null || model.Updates == null || !model.Updates.Any())
                {
                    return Json(new { success = false, message = "Không có dữ liệu cập nhật" });
                }

                var updates = model.Updates;

                // Log dữ liệu đầu vào để debug
                System.Diagnostics.Debug.WriteLine($"Received {updates.Count} updates");
                foreach (var update in updates)
                {
                    System.Diagnostics.Debug.WriteLine($"ImageId: {update.ImageId}, Order: {update.Order}");
                }

                var token = HttpContext.Session.GetString("Token");
                var results = new List<object>();
                var hasErrors = false;

                foreach (var update in updates)
                {
                    // Kiểm tra dữ liệu hợp lệ
                    if (string.IsNullOrEmpty(update.ImageId))
                    {
                        results.Add(new {
                            imageId = update.ImageId,
                            success = false,
                            message = "ImageId không hợp lệ"
                        });
                        hasErrors = true;
                        continue;
                    }

                    // Thêm log để debug
                    System.Diagnostics.Debug.WriteLine($"Processing update for imageId={update.ImageId}, order={update.Order}");

                    var updateDto = new UpdateImageOrderDTO { Order = update.Order.ToString() };
                    var response = await _productService.UpdateImageOrderAsync(update.ImageId, updateDto, token);

                    results.Add(new {
                        imageId = update.ImageId,
                        success = response.Success,
                        message = response.Message
                    });

                    if (!response.Success)
                    {
                        hasErrors = true;
                    }
                }

                return Json(new {
                    success = !hasErrors,
                    message = hasErrors ? "Có lỗi xảy ra khi cập nhật một số ảnh" : "Cập nhật thứ tự ảnh thành công",
                    results = results
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in UpdateMultipleImageOrders: {ex}");
                return Json(new {
                    success = false,
                    message = $"Lỗi: {ex.Message}",
                    stackTrace = ex.StackTrace // Thêm stack trace để debug
                });
            }
        }
    }
}