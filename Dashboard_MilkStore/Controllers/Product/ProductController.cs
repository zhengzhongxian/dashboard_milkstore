using Dashboard_MilkStore.Models.Product;
using Dashboard_MilkStore.Services.Category;
using Dashboard_MilkStore.Services.Product;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dashboard_MilkStore.Controllers.Product
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductController(IProductService productService, ICategoryService categoryService, IHttpContextAccessor httpContextAccessor)
        {
            _productService = productService;
            _categoryService = categoryService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Index(string? searchTerm = null, string? categoryId = null, string? sortBy = null, bool sortAscending = true, int pageNumber = 1, int pageSize = 10)
        {
            var request = new ProductQueryRequest
            {
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                SortBy = sortBy,
                SortAscending = sortAscending,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
            var response = await _productService.GetProductsAsync(request, token);

            // Lấy danh sách danh mục để hiển thị trong dropdown
            var categoriesResponse = await _categoryService.GetCategoriesAsync(token);
            ViewBag.Categories = categoriesResponse.Success && categoriesResponse.Data != null
                ? categoriesResponse.Data
                : new List<Models.Category.Category>();

            // Lưu các tham số tìm kiếm vào ViewBag để sử dụng trong view
            ViewBag.SearchTerm = searchTerm;
            ViewBag.CategoryId = categoryId;
            ViewBag.SortBy = sortBy;
            ViewBag.SortAscending = sortAscending;

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

        // GET: Product/Create
        public async Task<IActionResult> Create()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
            var categoriesResponse = await _categoryService.GetCategoriesAsync(token);

            var viewModel = new ProductCreateViewModel
            {
                IsActive = true,
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
                    }).ToList(),
                Categories = categoriesResponse.Success && categoriesResponse.Data != null
                    ? categoriesResponse.Data.Select(c => new SelectListItem
                    {
                        Value = c.Categoryid,
                        Text = c.CategoryName
                    }).ToList()
                    : new List<SelectListItem>()
            };

            return View(viewModel);
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel viewModel)
        {
            System.Diagnostics.Debug.WriteLine("Create action called");
            System.Diagnostics.Debug.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

            if (!ModelState.IsValid)
            {
                System.Diagnostics.Debug.WriteLine("ModelState is not valid");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    System.Diagnostics.Debug.WriteLine($"ModelState error: {error.ErrorMessage}");
                }

                // Nếu model không hợp lệ, tải lại các danh sách dropdown
                var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
                var categoriesResponse = await _categoryService.GetCategoriesAsync(token);

                viewModel.Statuses = (await _productService.GetProductStatus())
                    .Select(s => new SelectListItem
                    {
                        Value = s.StatusId,
                        Text = s.Name
                    }).ToList();
                viewModel.Brands = (await _productService.GetBrands())
                    .Select(b => new SelectListItem
                    {
                        Value = b.Brand1,
                        Text = b.BrandName,
                    }).ToList();
                viewModel.Units = (await _productService.GetUnits())
                    .Select(u => new SelectListItem
                    {
                        Value = u.Unit1,
                        Text = u.UnitName ?? u.Unit1,
                    }).ToList();
                viewModel.Categories = categoriesResponse.Success && categoriesResponse.Data != null
                    ? categoriesResponse.Data.Select(c => new SelectListItem
                    {
                        Value = c.Categoryid,
                        Text = c.CategoryName
                    }).ToList()
                    : new List<SelectListItem>();

                return View(viewModel);
            }

            System.Diagnostics.Debug.WriteLine($"ProductName: {viewModel.ProductName}");
            System.Diagnostics.Debug.WriteLine($"Brand1: {viewModel.Brand1}");
            System.Diagnostics.Debug.WriteLine($"Sku: {viewModel.Sku}");
            System.Diagnostics.Debug.WriteLine($"Bar: {viewModel.Bar}");
            System.Diagnostics.Debug.WriteLine($"StockQuantity: {viewModel.StockQuantity}");
            System.Diagnostics.Debug.WriteLine($"Unit1: {viewModel.Unit1}");
            System.Diagnostics.Debug.WriteLine($"StatusId: {viewModel.StatusId}");
            System.Diagnostics.Debug.WriteLine($"IsActive: {viewModel.IsActive}");
            System.Diagnostics.Debug.WriteLine($"Price: {viewModel.Price}");
            System.Diagnostics.Debug.WriteLine($"IsDefaultPrice: {viewModel.IsDefaultPrice}");
            System.Diagnostics.Debug.WriteLine($"IsActivePrice: {viewModel.IsActivePrice}");

            if (viewModel.Prices != null)
            {
                System.Diagnostics.Debug.WriteLine($"Prices count: {viewModel.Prices.Count}");
                foreach (var price in viewModel.Prices)
                {
                    System.Diagnostics.Debug.WriteLine($"Price: {price.Price}, IsDefault: {price.IsDefault}, IsActive: {price.IsActive}");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Prices is null");
            }

            if (viewModel.Images != null)
            {
                System.Diagnostics.Debug.WriteLine($"Images count: {viewModel.Images.Count}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Images is null");
            }

            if (viewModel.Dimensions != null)
            {
                System.Diagnostics.Debug.WriteLine($"Dimensions count: {viewModel.Dimensions.Count}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Dimensions is null");
            }

            try
            {
                // Chuyển đổi từ ViewModel sang DTO
                var createDto = new CreateProductDTO
                {
                    ProductName = viewModel.ProductName,
                    Brand = viewModel.Brand1,
                    Sku = viewModel.Sku,
                    Bar = viewModel.Bar,
                    StockQuantity = viewModel.StockQuantity ?? 0,
                    Unit = viewModel.Unit1,
                    Description = viewModel.Description,
                    StatusId = viewModel.StatusId,
                    IsActive = viewModel.IsActive,
                    Metadata = null,
                    CategoryIds = viewModel.CategoryIds ?? new List<string>(),
                    ProductPrices = new List<CreateProductPriceDTO>(),
                    Images = new List<CreateImageDTONew>(),
                    Dimensions = new List<CreateDimensionDTONew>()
                };

                // Thêm giá sản phẩm nếu có
                if (viewModel.Price.HasValue)
                {
                    createDto.ProductPrices.Add(new CreateProductPriceDTO
                    {
                        Price = viewModel.Price.Value,
                        IsDefault = viewModel.IsDefaultPrice,
                        IsActive = viewModel.IsActivePrice
                    });
                }

                // Thêm các giá từ danh sách Prices nếu có
                if (viewModel.Prices != null && viewModel.Prices.Any())
                {
                    foreach (var price in viewModel.Prices)
                    {
                        createDto.ProductPrices.Add(new CreateProductPriceDTO
                        {
                            Price = price.Price,
                            IsDefault = price.IsDefault,
                            IsActive = price.IsActive
                        });
                    }
                }

                // Thêm kích thước nếu có
                if (viewModel.LengthValue.HasValue || viewModel.WidthValue.HasValue ||
                    viewModel.HeightValue.HasValue || viewModel.WeightValue.HasValue)
                {
                    createDto.Dimensions.Add(new CreateDimensionDTONew
                    {
                        LengthValue = viewModel.LengthValue,
                        WidthValue = viewModel.WidthValue,
                        HeightValue = viewModel.HeightValue,
                        WeightValue = viewModel.WeightValue
                    });
                }

                // Thêm các kích thước từ danh sách Dimensions nếu có
                if (viewModel.Dimensions != null && viewModel.Dimensions.Any())
                {
                    foreach (var dimension in viewModel.Dimensions)
                    {
                        createDto.Dimensions.Add(new CreateDimensionDTONew
                        {
                            LengthValue = dimension.LengthValue,
                            WidthValue = dimension.WidthValue,
                            HeightValue = dimension.HeightValue,
                            WeightValue = dimension.WeightValue
                        });
                    }
                }

                // Thêm các hình ảnh từ danh sách Images nếu có
                if (viewModel.Images != null && viewModel.Images.Any())
                {
                    foreach (var image in viewModel.Images)
                    {
                        createDto.Images.Add(new CreateImageDTONew
                        {
                            ImageData = image.ImageData,
                            Order = image.Order
                        });
                    }
                }

                // Lấy token từ session
                var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");

                // Gọi API để tạo sản phẩm
                System.Diagnostics.Debug.WriteLine("Calling CreateProductAsync...");
                var response = await _productService.CreateProductAsync(createDto, token);
                System.Diagnostics.Debug.WriteLine($"CreateProductAsync response: {(response != null ? "not null" : "null")}");

                if (response != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Response Success: {response.Success}");
                    System.Diagnostics.Debug.WriteLine($"Response Message: {response.Message}");
                    System.Diagnostics.Debug.WriteLine($"Response StatusCode: {response.StatusCode}");
                    System.Diagnostics.Debug.WriteLine($"Response Data: {(response.Data != null ? "not null" : "null")}");

                    if (response.Data != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Product ID: {response.Data.ProductId}");
                    }
                }

                if (response == null || !response.Success)
                {
                    System.Diagnostics.Debug.WriteLine("Response is null or not successful");
                    ModelState.AddModelError("", response?.Message ?? "Có lỗi xảy ra khi tạo sản phẩm");

                    // Tải lại các danh sách dropdown
                    var categoriesResponse = await _categoryService.GetCategoriesAsync(token);

                    viewModel.Statuses = (await _productService.GetProductStatus())
                        .Select(s => new SelectListItem
                        {
                            Value = s.StatusId,
                            Text = s.Name
                        }).ToList();
                    viewModel.Brands = (await _productService.GetBrands())
                        .Select(b => new SelectListItem
                        {
                            Value = b.Brand1,
                            Text = b.BrandName,
                        }).ToList();
                    viewModel.Units = (await _productService.GetUnits())
                        .Select(u => new SelectListItem
                        {
                            Value = u.Unit1,
                            Text = u.UnitName ?? u.Unit1,
                        }).ToList();
                    viewModel.Categories = categoriesResponse.Success && categoriesResponse.Data != null
                        ? categoriesResponse.Data.Select(c => new SelectListItem
                        {
                            Value = c.Categoryid,
                            Text = c.CategoryName
                        }).ToList()
                        : new List<SelectListItem>();

                    return View(viewModel);
                }

                // Chuyển hướng đến trang chỉnh sửa sản phẩm để thêm hình ảnh và giá
                System.Diagnostics.Debug.WriteLine($"Product created successfully with ID: {response.Data?.ProductId}");
                TempData["SuccessMessage"] = "Tạo sản phẩm thành công! Bạn có thể thêm hình ảnh và giá cho sản phẩm.";
                return RedirectToAction("Edit", new { id = response.Data.ProductId });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in Create action: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    ModelState.AddModelError("", $"Lỗi: {ex.Message}. Chi tiết: {ex.InnerException.Message}");
                }
                else
                {
                    ModelState.AddModelError("", $"Lỗi: {ex.Message}");
                }

                // Tải lại các danh sách dropdown
                var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
                var categoriesResponse = await _categoryService.GetCategoriesAsync(token);

                viewModel.Statuses = (await _productService.GetProductStatus())
                    .Select(s => new SelectListItem
                    {
                        Value = s.StatusId,
                        Text = s.Name
                    }).ToList();
                viewModel.Brands = (await _productService.GetBrands())
                    .Select(b => new SelectListItem
                    {
                        Value = b.Brand1,
                        Text = b.BrandName,
                    }).ToList();
                viewModel.Units = (await _productService.GetUnits())
                    .Select(u => new SelectListItem
                    {
                        Value = u.Unit1,
                        Text = u.UnitName ?? u.Unit1,
                    }).ToList();
                viewModel.Categories = categoriesResponse.Success && categoriesResponse.Data != null
                    ? categoriesResponse.Data.Select(c => new SelectListItem
                    {
                        Value = c.Categoryid,
                        Text = c.CategoryName
                    }).ToList()
                    : new List<SelectListItem>();

                return View(viewModel);
            }
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(string id, string? searchTerm = null, string? categoryId = null,
            string? sortBy = null, bool sortAscending = true, int pageNumber = 1, int pageSize = 10)
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

            // Lưu các tham số tìm kiếm vào ViewBag để sử dụng trong view
            ViewBag.SearchTerm = searchTerm;
            ViewBag.CategoryId = categoryId;
            ViewBag.SortBy = sortBy;
            ViewBag.SortAscending = sortAscending;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;

            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
            var categoriesResponse = await _categoryService.GetCategoriesAsync(token);
            var productCategoriesResponse = await _productService.GetProductCategoriesAsync(product.ProductId, token);

            // Log để debug
            System.Diagnostics.Debug.WriteLine($"Product categories response: Success={productCategoriesResponse?.Success}, Count={productCategoriesResponse?.Data?.Count ?? 0}");
            if (productCategoriesResponse?.Data != null)
            {
                foreach (var category in productCategoriesResponse.Data)
                {
                    System.Diagnostics.Debug.WriteLine($"Category: ID={category.CategoryId}, Name={category.CategoryName}");
                }
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
                    }).ToList(),
                Categories = categoriesResponse.Success && categoriesResponse.Data != null
                    ? categoriesResponse.Data.Select(c => new SelectListItem
                    {
                        Value = c.Categoryid,
                        Text = c.CategoryName
                    }).ToList()
                    : new List<SelectListItem>(),
                ProductCategories = productCategoriesResponse.Success && productCategoriesResponse.Data != null
                    ? productCategoriesResponse.Data.Select(pc => new ProductCategoryViewModel
                    {
                        Id = pc.Id,
                        ProductId = pc.ProductId,
                        CategoryId = pc.CategoryId,
                        CategoryName = pc.CategoryName
                    }).ToList()
                    : new List<ProductCategoryViewModel>()
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

        // Dimension operations
        [HttpPost]
        public async Task<IActionResult> AddDimension([FromBody] AddDimensionRequest request)
        {
            if (string.IsNullOrEmpty(request.ProductId))
            {
                return Json(new { success = false, message = "ProductId không được để trống" });
            }

            try
            {
                var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");

                var createDto = new CreateDimensionDTO
                {
                    ProductId = request.ProductId,
                    WeightValue = request.WeightValue,
                    HeightValue = request.HeightValue,
                    WidthValue = request.WidthValue,
                    LengthValue = request.LengthValue,
                    Metadata = request.Metadata
                };

                var response = await _productService.AddDimensionAsync(createDto, token);

                if (response.Success)
                {
                    return Json(new {
                        success = true,
                        message = "Thêm kích thước thành công",
                        data = response.Data
                    });
                }
                else
                {
                    return Json(new {
                        success = false,
                        message = response.Message
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in AddDimension: {ex.Message}");
                return Json(new {
                    success = false,
                    message = $"Lỗi khi thêm kích thước: {ex.Message}"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDimension([FromBody] UpdateDimensionRequest request)
        {
            System.Diagnostics.Debug.WriteLine($"UpdateDimension called with request: {System.Text.Json.JsonSerializer.Serialize(request)}");

            if (string.IsNullOrEmpty(request.DimensionId))
            {
                System.Diagnostics.Debug.WriteLine("UpdateDimension: DimensionId is empty");
                return Json(new { success = false, message = "DimensionId không được để trống" });
            }

            try
            {
                var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
                System.Diagnostics.Debug.WriteLine($"UpdateDimension: Token from session: {(string.IsNullOrEmpty(token) ? "null" : "present")}");

                var patchValues = new Dictionary<string, object>();

                if (request.WeightValue.HasValue)
                {
                    patchValues.Add("weightValue", request.WeightValue.Value);
                    System.Diagnostics.Debug.WriteLine($"UpdateDimension: Adding weightValue: {request.WeightValue.Value}");
                }

                if (request.HeightValue.HasValue)
                {
                    patchValues.Add("heightValue", request.HeightValue.Value);
                    System.Diagnostics.Debug.WriteLine($"UpdateDimension: Adding heightValue: {request.HeightValue.Value}");
                }

                if (request.WidthValue.HasValue)
                {
                    patchValues.Add("widthValue", request.WidthValue.Value);
                    System.Diagnostics.Debug.WriteLine($"UpdateDimension: Adding widthValue: {request.WidthValue.Value}");
                }

                if (request.LengthValue.HasValue)
                {
                    patchValues.Add("lengthValue", request.LengthValue.Value);
                    System.Diagnostics.Debug.WriteLine($"UpdateDimension: Adding lengthValue: {request.LengthValue.Value}");
                }

                if (!string.IsNullOrEmpty(request.Metadata))
                {
                    patchValues.Add("metadata", request.Metadata);
                    System.Diagnostics.Debug.WriteLine($"UpdateDimension: Adding metadata: {request.Metadata}");
                }

                System.Diagnostics.Debug.WriteLine($"UpdateDimension: Calling UpdateDimensionAsync with dimensionId: {request.DimensionId} and patchValues: {System.Text.Json.JsonSerializer.Serialize(patchValues)}");
                var response = await _productService.UpdateDimensionAsync(request.DimensionId, patchValues, token);
                System.Diagnostics.Debug.WriteLine($"UpdateDimension: Response from UpdateDimensionAsync: {System.Text.Json.JsonSerializer.Serialize(response)}");

                return Json(new {
                    success = response.Success,
                    message = response.Success ? "Cập nhật kích thước thành công" : response.Message
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in UpdateDimension: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Exception stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return Json(new {
                    success = false,
                    message = $"Lỗi khi cập nhật kích thước: {ex.Message}"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDimension([FromBody] DeleteDimensionRequest request)
        {
            if (string.IsNullOrEmpty(request.DimensionId))
            {
                return Json(new { success = false, message = "DimensionId không được để trống" });
            }

            try
            {
                var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");

                var response = await _productService.DeleteDimensionAsync(request.DimensionId, token);

                return Json(new {
                    success = response.Success,
                    message = response.Success ? "Xóa kích thước thành công" : response.Message
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in DeleteDimension: {ex.Message}");
                return Json(new {
                    success = false,
                    message = $"Lỗi khi xóa kích thước: {ex.Message}"
                });
            }
        }

        // Model classes for dimension operations
        public class AddDimensionRequest
        {
            public string ProductId { get; set; }
            public decimal? WeightValue { get; set; }
            public decimal? HeightValue { get; set; }
            public decimal? WidthValue { get; set; }
            public decimal? LengthValue { get; set; }
            public string? Metadata { get; set; }
        }

        public class UpdateDimensionRequest
        {
            public string DimensionId { get; set; }
            public decimal? WeightValue { get; set; }
            public decimal? HeightValue { get; set; }
            public decimal? WidthValue { get; set; }
            public decimal? LengthValue { get; set; }
            public string? Metadata { get; set; }
        }

        public class DeleteDimensionRequest
        {
            public string DimensionId { get; set; }
        }

        // ProductPrice operations
        [HttpPost]
        public async Task<IActionResult> AddProductPrice([FromBody] AddProductPriceRequest request)
        {
            if (string.IsNullOrEmpty(request.ProductId))
            {
                return Json(new { success = false, message = "ProductId không được để trống" });
            }

            try
            {
                var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");

                var createDto = new CreateProductPriceDTONew
                {
                    ProductId = request.ProductId,
                    Price = request.Price,
                    IsDefault = request.IsDefault,
                    IsActive = request.IsActive
                };

                var response = await _productService.AddProductPriceAsync(createDto, token);

                if (response.Success)
                {
                    return Json(new {
                        success = true,
                        message = "Thêm giá sản phẩm thành công",
                        data = response.Data
                    });
                }
                else
                {
                    return Json(new {
                        success = false,
                        message = response.Message
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in AddProductPrice: {ex.Message}");
                return Json(new {
                    success = false,
                    message = $"Lỗi khi thêm giá sản phẩm: {ex.Message}"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProductPrice([FromBody] UpdateProductPriceRequest request)
        {
            System.Diagnostics.Debug.WriteLine($"UpdateProductPrice called with request: {System.Text.Json.JsonSerializer.Serialize(request)}");

            if (string.IsNullOrEmpty(request.PriceId))
            {
                System.Diagnostics.Debug.WriteLine("UpdateProductPrice: PriceId is empty");
                return Json(new { success = false, message = "PriceId không được để trống" });
            }

            try
            {
                var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
                System.Diagnostics.Debug.WriteLine($"UpdateProductPrice: Token from session: {(string.IsNullOrEmpty(token) ? "null" : "present")}");

                var patchValues = new Dictionary<string, object>();

                if (request.Price.HasValue)
                {
                    patchValues.Add("price", request.Price.Value);
                    System.Diagnostics.Debug.WriteLine($"UpdateProductPrice: Adding price: {request.Price.Value}");
                }

                if (request.IsDefault.HasValue)
                {
                    patchValues.Add("isDefault", request.IsDefault.Value);
                    System.Diagnostics.Debug.WriteLine($"UpdateProductPrice: Adding isDefault: {request.IsDefault.Value}");
                }

                if (request.IsActive.HasValue)
                {
                    patchValues.Add("isActive", request.IsActive.Value);
                    System.Diagnostics.Debug.WriteLine($"UpdateProductPrice: Adding isActive: {request.IsActive.Value}");
                }

                System.Diagnostics.Debug.WriteLine($"UpdateProductPrice: Calling UpdateProductPriceAsync with priceId: {request.PriceId} and patchValues: {System.Text.Json.JsonSerializer.Serialize(patchValues)}");
                var response = await _productService.UpdateProductPriceAsync(request.PriceId, patchValues, token);
                System.Diagnostics.Debug.WriteLine($"UpdateProductPrice: Response from UpdateProductPriceAsync: {System.Text.Json.JsonSerializer.Serialize(response)}");

                return Json(new {
                    success = response.Success,
                    message = response.Success ? "Cập nhật giá sản phẩm thành công" : response.Message
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in UpdateProductPrice: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Exception stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                return Json(new {
                    success = false,
                    message = $"Lỗi khi cập nhật giá sản phẩm: {ex.Message}"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProductPrice([FromBody] DeleteProductPriceRequest request)
        {
            if (string.IsNullOrEmpty(request.PriceId))
            {
                return Json(new { success = false, message = "PriceId không được để trống" });
            }

            try
            {
                var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");

                var response = await _productService.DeleteProductPriceAsync(request.PriceId, token);

                return Json(new {
                    success = response.Success,
                    message = response.Success ? "Xóa giá sản phẩm thành công" : response.Message
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in DeleteProductPrice: {ex.Message}");
                return Json(new {
                    success = false,
                    message = $"Lỗi khi xóa giá sản phẩm: {ex.Message}"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetDefaultPrice([FromBody] SetDefaultPriceRequest request)
        {
            if (string.IsNullOrEmpty(request.PriceId))
            {
                return Json(new { success = false, message = "PriceId không được để trống" });
            }

            try
            {
                var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");

                var response = await _productService.SetDefaultPriceAsync(request.PriceId, request.IsDefault, token);

                return Json(new {
                    success = response.Success,
                    message = response.Success ? "Cập nhật giá mặc định thành công" : response.Message
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in SetDefaultPrice: {ex.Message}");
                return Json(new {
                    success = false,
                    message = $"Lỗi khi cập nhật giá mặc định: {ex.Message}"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetPriceStatus([FromBody] SetPriceStatusRequest request)
        {
            if (string.IsNullOrEmpty(request.PriceId))
            {
                return Json(new { success = false, message = "PriceId không được để trống" });
            }

            try
            {
                var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");

                var response = await _productService.SetPriceStatusAsync(request.PriceId, request.IsActive, token);

                return Json(new {
                    success = response.Success,
                    message = response.Success ? "Cập nhật trạng thái giá thành công" : response.Message
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in SetPriceStatus: {ex.Message}");
                return Json(new {
                    success = false,
                    message = $"Lỗi khi cập nhật trạng thái giá: {ex.Message}"
                });
            }
        }

        // Model classes for ProductPrice operations
        public class AddProductPriceRequest
        {
            public string ProductId { get; set; }
            public decimal Price { get; set; }
            public bool IsDefault { get; set; }
            public bool IsActive { get; set; } = true;
        }

        public class UpdateProductPriceRequest
        {
            public string PriceId { get; set; }
            public decimal? Price { get; set; }
            public bool? IsDefault { get; set; }
            public bool? IsActive { get; set; }
        }

        public class DeleteProductPriceRequest
        {
            public string PriceId { get; set; }
        }

        public class SetDefaultPriceRequest
        {
            public string PriceId { get; set; }
            public bool IsDefault { get; set; }
        }

        public class SetPriceStatusRequest
        {
            public string PriceId { get; set; }
            public bool IsActive { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPriceChanges(string productId, [FromBody] ProductPriceChangesViewModel changes)
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

                // 1. Process deleted prices
                if (changes.DeletedPriceIds != null && changes.DeletedPriceIds.Any())
                {
                    foreach (var priceId in changes.DeletedPriceIds)
                    {
                        var deleteResponse = await _productService.DeleteProductPriceAsync(priceId, token);
                        if (deleteResponse.Success)
                        {
                            successCount++;
                        }
                        else
                        {
                            errorMessages.Add($"Lỗi khi xóa giá {priceId}: {deleteResponse.Message}");
                        }
                    }
                }

                // 2. Process new prices
                if (changes.NewPrices != null && changes.NewPrices.Any())
                {
                    foreach (var newPrice in changes.NewPrices)
                    {
                        try
                        {
                            var createDto = new CreateProductPriceDTONew
                            {
                                ProductId = productId,
                                Price = newPrice.Price,
                                IsDefault = newPrice.IsDefault,
                                IsActive = newPrice.IsActive
                            };

                            var addResponse = await _productService.AddProductPriceAsync(createDto, token);
                            if (addResponse.Success)
                            {
                                successCount++;
                            }
                            else
                            {
                                errorMessages.Add($"Lỗi khi thêm giá mới: {addResponse.Message}");
                            }
                        }
                        catch (Exception ex)
                        {
                            errorMessages.Add($"Lỗi xử lý giá: {ex.Message}");
                        }
                    }
                }

                // 3. Process updated prices
                if (changes.UpdatedPrices != null && changes.UpdatedPrices.Any())
                {
                    foreach (var update in changes.UpdatedPrices)
                    {
                        var patchValues = new Dictionary<string, object>();

                        if (update.Price.HasValue)
                        {
                            patchValues.Add("price", update.Price.Value);
                        }

                        if (update.IsDefault.HasValue)
                        {
                            patchValues.Add("isDefault", update.IsDefault.Value);
                        }

                        if (update.IsActive.HasValue)
                        {
                            patchValues.Add("isActive", update.IsActive.Value);
                        }

                        var updateResponse = await _productService.UpdateProductPriceAsync(update.PriceId, patchValues, token);
                        if (updateResponse.Success)
                        {
                            successCount++;
                        }
                        else
                        {
                            errorMessages.Add($"Lỗi khi cập nhật giá {update.PriceId}: {updateResponse.Message}");
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
                        message = $"Đã cập nhật {successCount} thay đổi giá thành công"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi cập nhật giá: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveDimensionChanges(string productId,
            [FromBody] ProductDimensionChangesViewModel changes)
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

                // 1. Process updated dimensions
                if (changes.UpdatedDimensions != null && changes.UpdatedDimensions.Any())
                {
                    foreach (var dimension in changes.UpdatedDimensions)
                    {
                        var patchValues = new Dictionary<string, object>();

                        if (dimension.LengthValue.HasValue)
                            patchValues.Add("lengthValue", dimension.LengthValue.Value);

                        if (dimension.WidthValue.HasValue)
                            patchValues.Add("widthValue", dimension.WidthValue.Value);

                        if (dimension.HeightValue.HasValue)
                            patchValues.Add("heightValue", dimension.HeightValue.Value);

                        if (dimension.WeightValue.HasValue)
                            patchValues.Add("weightValue", dimension.WeightValue.Value);

                        if (!string.IsNullOrEmpty(dimension.Metadata))
                            patchValues.Add("metadata", dimension.Metadata);

                        if (patchValues.Any())
                        {
                            var updateResponse = await _productService.UpdateDimensionAsync(dimension.DimensionId, patchValues, token);
                            if (updateResponse.Success)
                            {
                                successCount++;
                            }
                            else
                            {
                                errorMessages.Add($"Lỗi khi cập nhật kích thước {dimension.DimensionId}: {updateResponse.Message}");
                            }
                        }
                    }
                }

                // 2. Process deleted dimensions
                if (changes.DeletedDimensionIds != null && changes.DeletedDimensionIds.Any())
                {
                    foreach (var dimensionId in changes.DeletedDimensionIds)
                    {
                        var deleteResponse = await _productService.DeleteDimensionAsync(dimensionId, token);
                        if (deleteResponse.Success)
                        {
                            successCount++;
                        }
                        else
                        {
                            errorMessages.Add($"Lỗi khi xóa kích thước {dimensionId}: {deleteResponse.Message}");
                        }
                    }
                }

                // 3. Process new dimensions
                if (changes.NewDimensions != null && changes.NewDimensions.Any())
                {
                    foreach (var dimension in changes.NewDimensions)
                    {
                        var createDto = new CreateDimensionDTO
                        {
                            ProductId = productId,
                            LengthValue = dimension.LengthValue,
                            WidthValue = dimension.WidthValue,
                            HeightValue = dimension.HeightValue,
                            WeightValue = dimension.WeightValue,
                            Metadata = dimension.Metadata
                        };

                        var addResponse = await _productService.AddDimensionAsync(createDto, token);
                        if (addResponse.Success)
                        {
                            successCount++;
                        }
                        else
                        {
                            errorMessages.Add($"Lỗi khi thêm kích thước mới: {addResponse.Message}");
                        }
                    }
                }

                // Return result
                if (errorMessages.Any())
                {
                    return Json(new
                    {
                        success = successCount > 0,
                        message = $"Đã xử lý {successCount} thay đổi thành công. Lỗi: {string.Join("; ", errorMessages)}"
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = true,
                        message = $"Đã xử lý {successCount} thay đổi thành công"
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi cập nhật kích thước: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveProductCategoryChanges(string productId,
            [FromBody] dynamic data)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return Json(new { success = false, message = "ID sản phẩm không hợp lệ" });
            }

            try
            {
                // Log dữ liệu nhận được
                System.Diagnostics.Debug.WriteLine($"SaveProductCategoryChanges: Received data for product {productId}");
                System.Diagnostics.Debug.WriteLine($"SaveProductCategoryChanges: Data type: {data?.GetType().Name}");

                // Trích xuất changes từ data
                ProductCategoryChangesViewModel changes;

                if (data.changes != null)
                {
                    // Nếu data có thuộc tính changes
                    changes = new ProductCategoryChangesViewModel
                    {
                        DeletedCategoryIds = data.changes.deletedCategoryIds?.ToObject<List<string>>() ?? new List<string>(),
                        NewCategoryIds = data.changes.newCategoryIds?.ToObject<List<string>>() ?? new List<string>()
                    };
                    System.Diagnostics.Debug.WriteLine($"SaveProductCategoryChanges: Extracted from data.changes");
                }
                else
                {
                    // Nếu data là changes
                    changes = new ProductCategoryChangesViewModel
                    {
                        DeletedCategoryIds = data.deletedCategoryIds?.ToObject<List<string>>() ?? new List<string>(),
                        NewCategoryIds = data.newCategoryIds?.ToObject<List<string>>() ?? new List<string>()
                    };
                    System.Diagnostics.Debug.WriteLine($"SaveProductCategoryChanges: Extracted directly from data");
                }

                System.Diagnostics.Debug.WriteLine($"SaveProductCategoryChanges: Deleted categories: {changes.DeletedCategoryIds?.Count ?? 0}");
                System.Diagnostics.Debug.WriteLine($"SaveProductCategoryChanges: New categories: {changes.NewCategoryIds?.Count ?? 0}");

                var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
                System.Diagnostics.Debug.WriteLine($"SaveProductCategoryChanges: Token present: {!string.IsNullOrEmpty(token)}");

                var response = await _productService.ProcessProductCategoryChangesAsync(productId, changes, token);

                return Json(new {
                    success = response.Success,
                    message = response.Message,
                    data = new {
                        deletedCount = changes.DeletedCategoryIds?.Count ?? 0,
                        newCount = changes.NewCategoryIds?.Count ?? 0
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveProductCategoryChanges: Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"SaveProductCategoryChanges: Stack trace: {ex.StackTrace}");
                return Json(new { success = false, message = $"Lỗi khi cập nhật danh mục sản phẩm: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProductCategories(string productId)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return Json(new { success = false, message = "ID sản phẩm không hợp lệ" });
            }

            try
            {
                var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
                var response = await _productService.GetProductCategoriesAsync(productId, token);

                return Json(new {
                    success = response.Success,
                    message = response.Message,
                    data = response.Data
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi lấy danh mục sản phẩm: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProductCategory([FromBody] AddProductCategoryRequest request)
        {
            if (string.IsNullOrEmpty(request.ProductId) || string.IsNullOrEmpty(request.CategoryId))
            {
                return Json(new { success = false, message = "ProductId và CategoryId không được để trống" });
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"AddProductCategory: ProductId={request.ProductId}, CategoryId={request.CategoryId}");

                var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");

                var createDto = new CreateProductCategoryViewModel
                {
                    ProductId = request.ProductId,
                    CategoryId = request.CategoryId
                };

                var response = await _productService.AddProductCategoryAsync(createDto, token);

                System.Diagnostics.Debug.WriteLine($"AddProductCategory: Response success={response.Success}, message={response.Message}");

                return Json(new {
                    success = response.Success,
                    message = response.Success ? "Thêm danh mục vào sản phẩm thành công" : response.Message,
                    data = response.Data
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddProductCategory: Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"AddProductCategory: Stack trace: {ex.StackTrace}");
                return Json(new { success = false, message = $"Lỗi khi thêm danh mục vào sản phẩm: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProductCategory([FromBody] DeleteProductCategoryRequest request)
        {
            if (string.IsNullOrEmpty(request.ProductId) || string.IsNullOrEmpty(request.CategoryId))
            {
                return Json(new { success = false, message = "ProductId và CategoryId không được để trống" });
            }

            try
            {
                System.Diagnostics.Debug.WriteLine($"DeleteProductCategory: ProductId={request.ProductId}, CategoryId={request.CategoryId}");

                var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");

                var response = await _productService.DeleteProductCategoryAsync(request.ProductId, request.CategoryId, token);

                System.Diagnostics.Debug.WriteLine($"DeleteProductCategory: Response success={response.Success}, message={response.Message}");

                return Json(new {
                    success = response.Success,
                    message = response.Success ? "Xóa danh mục khỏi sản phẩm thành công" : response.Message
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteProductCategory: Exception: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"DeleteProductCategory: Stack trace: {ex.StackTrace}");
                return Json(new { success = false, message = $"Lỗi khi xóa danh mục khỏi sản phẩm: {ex.Message}" });
            }
        }

        // Model classes for ProductCategory operations
        public class AddProductCategoryRequest
        {
            public string ProductId { get; set; }
            public string CategoryId { get; set; }
        }

        public class DeleteProductCategoryRequest
        {
            public string ProductId { get; set; }
            public string CategoryId { get; set; }
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(string id, string? searchTerm = null, string? categoryId = null,
            string? sortBy = null, bool sortAscending = true, int pageNumber = 1, int pageSize = 10)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID sản phẩm không hợp lệ";
                return RedirectToAction(nameof(Index), new {
                    searchTerm = searchTerm,
                    categoryId = categoryId,
                    sortBy = sortBy,
                    sortAscending = sortAscending,
                    pageNumber = pageNumber,
                    pageSize = pageSize
                });
            }

            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy sản phẩm";
                    return RedirectToAction(nameof(Index), new {
                        searchTerm = searchTerm,
                        categoryId = categoryId,
                        sortBy = sortBy,
                        sortAscending = sortAscending,
                        pageNumber = pageNumber,
                        pageSize = pageSize
                    });
                }

                // Lưu các tham số tìm kiếm vào ViewBag để sử dụng trong view
                ViewBag.SearchTerm = searchTerm;
                ViewBag.CategoryId = categoryId;
                ViewBag.SortBy = sortBy;
                ViewBag.SortAscending = sortAscending;
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;

                // Hiển thị trang xác nhận xóa
                return View(product);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi tải thông tin sản phẩm: {ex.Message}";
                return RedirectToAction(nameof(Index), new {
                    searchTerm = searchTerm,
                    categoryId = categoryId,
                    sortBy = sortBy,
                    sortAscending = sortAscending,
                    pageNumber = pageNumber,
                    pageSize = pageSize
                });
            }
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id, string? searchTerm = null, string? categoryId = null,
            string? sortBy = null, bool sortAscending = true, int pageNumber = 1, int pageSize = 10)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "ID sản phẩm không hợp lệ";
                return RedirectToAction(nameof(Index), new {
                    searchTerm = searchTerm,
                    categoryId = categoryId,
                    sortBy = sortBy,
                    sortAscending = sortAscending,
                    pageNumber = pageNumber,
                    pageSize = pageSize
                });
            }

            try
            {
                var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
                var response = await _productService.DeleteProductAsync(id, token);

                if (response.Success)
                {
                    TempData["SuccessMessage"] = "Xóa sản phẩm thành công";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Lỗi khi xóa sản phẩm: {response.Message}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi xóa sản phẩm: {ex.Message}";
            }

            return RedirectToAction(nameof(Index), new {
                searchTerm = searchTerm,
                categoryId = categoryId,
                sortBy = sortBy,
                sortAscending = sortAscending,
                pageNumber = pageNumber,
                pageSize = pageSize
            });
        }
    }
}