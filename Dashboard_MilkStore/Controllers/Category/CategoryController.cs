using Dashboard_MilkStore.Models.Category;
using Dashboard_MilkStore.Services.Category;
using Dashboard_MilkStore.Services.Parent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Dashboard_MilkStore.Controllers.Category
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IParentService _parentService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, IParentService parentService, IHttpContextAccessor httpContextAccessor, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _parentService = parentService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
            var response = await _categoryService.GetCategoriesAsync(token);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message;
                return View(new List<Models.Category.Category>());
            }

            return View(response.Data);
        }

        // GET: Category/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
            var response = await _categoryService.GetCategoryByIdAsync(id, token);

            if (!response.Success || response.Data == null)
            {
                TempData["ErrorMessage"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(response.Data);
        }

        // GET: Category/Create
        public async Task<IActionResult> Create()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
            var parentsResponse = await _parentService.GetParentsAsync(token);

            var viewModel = new CreateCategoryViewModel
            {
                Parents = parentsResponse.Success && parentsResponse.Data != null
                    ? parentsResponse.Data.Select(p => new SelectListItem
                    {
                        Value = p.Parentid,
                        Text = p.ParentName
                    }).ToList()
                    : new List<SelectListItem>()
            };

            return View(viewModel);
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
                var parentsResponse = await _parentService.GetParentsAsync(token);

                viewModel.Parents = parentsResponse.Success && parentsResponse.Data != null
                    ? parentsResponse.Data.Select(p => new SelectListItem
                    {
                        Value = p.Parentid,
                        Text = p.ParentName
                    }).ToList()
                    : new List<SelectListItem>();

                return View(viewModel);
            }

            var createToken = _httpContextAccessor.HttpContext?.Session.GetString("Token");
            var response = await _categoryService.CreateCategoryAsync(viewModel, createToken);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message;

                var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
                var parentsResponse = await _parentService.GetParentsAsync(token);

                viewModel.Parents = parentsResponse.Success && parentsResponse.Data != null
                    ? parentsResponse.Data.Select(p => new SelectListItem
                    {
                        Value = p.Parentid,
                        Text = p.ParentName
                    }).ToList()
                    : new List<SelectListItem>();

                return View(viewModel);
            }

            TempData["SuccessMessage"] = "Danh mục đã được tạo thành công.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
            var categoryResponse = await _categoryService.GetCategoryByIdAsync(id, token);
            var parentsResponse = await _parentService.GetParentsAsync(token);

            if (!categoryResponse.Success || categoryResponse.Data == null)
            {
                TempData["ErrorMessage"] = categoryResponse.Message;
                return RedirectToAction(nameof(Index));
            }

            var category = categoryResponse.Data;
            var viewModel = new CreateCategoryViewModel
            {
                CategoryName = category.CategoryName ?? string.Empty,
                Priority = category.Priority,
                Parentid = category.Parentid,
                Metadata = category.Metadata,
                Parents = parentsResponse.Success && parentsResponse.Data != null
                    ? parentsResponse.Data
                        .Select(p => new SelectListItem
                        {
                            Value = p.Parentid,
                            Text = p.ParentName,
                            Selected = p.Parentid == category.Parentid
                        }).ToList()
                    : new List<SelectListItem>()
            };

            return View(viewModel);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, CreateCategoryViewModel viewModel)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var sessionToken = _httpContextAccessor.HttpContext?.Session.GetString("Token");
                var parentsResponse = await _parentService.GetParentsAsync(sessionToken);

                viewModel.Parents = parentsResponse.Success && parentsResponse.Data != null
                    ? parentsResponse.Data
                        .Select(p => new SelectListItem
                        {
                            Value = p.Parentid,
                            Text = p.ParentName,
                            Selected = p.Parentid == viewModel.Parentid
                        }).ToList()
                    : new List<SelectListItem>();

                return View(viewModel);
            }

            var patchValues = new Dictionary<string, object>
            {
                { "CategoryName", viewModel.CategoryName },
                { "Priority", viewModel.Priority ?? 0 },
                { "Parentid", viewModel.Parentid ?? string.Empty },
                { "Metadata", viewModel.Metadata ?? string.Empty }
            };

            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
            var response = await _categoryService.UpdateCategoryAsync(id, patchValues, token);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message;

                var parentsResponse = await _parentService.GetParentsAsync(token);
                viewModel.Parents = parentsResponse.Success && parentsResponse.Data != null
                    ? parentsResponse.Data
                        .Select(p => new SelectListItem
                        {
                            Value = p.Parentid,
                            Text = p.ParentName,
                            Selected = p.Parentid == viewModel.Parentid
                        }).ToList()
                    : new List<SelectListItem>();

                return View(viewModel);
            }

            TempData["SuccessMessage"] = "Danh mục đã được cập nhật thành công.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
            var response = await _categoryService.GetCategoryByIdAsync(id, token);

            if (!response.Success || response.Data == null)
            {
                TempData["ErrorMessage"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            return View(response.Data);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
            var response = await _categoryService.DeleteCategoryAsync(id, token);

            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Danh mục đã được xóa thành công.";
            return RedirectToAction(nameof(Index));
        }
    }
}
