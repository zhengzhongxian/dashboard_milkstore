using Dashboard_MilkStore.Models.Account;
using Dashboard_MilkStore.Services.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard_MilkStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            IAuthService authService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AccountController> logger)
        {
            _authService = authService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // Check if user is already logged in
            var token = HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token) && !_authService.IsTokenExpired(token))
            {
                return RedirectToAction("Index", "Home");
            }

            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            // Kiểm tra validation phía client
            if (!ModelState.IsValid)
            {
                // Lấy tất cả lỗi validation và hiển thị
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                if (errors.Any())
                {
                    model.ErrorMessage = string.Join(", ", errors);
                }

                return View(model);
            }

            try
            {
                // Gọi API đăng nhập
                var response = await _authService.AdminLoginAsync(model.Username, model.Password);

                // Log phản hồi để debug
                _logger.LogInformation("Login response: Success={Success}, Message={Message}, StatusCode={StatusCode}",
                    response.Success, response.Message, response.StatusCode);

                if (response.Success && !string.IsNullOrEmpty(response.AccessToken))
                {
                    // Lưu token vào session
                    HttpContext.Session.SetString("Token", response.AccessToken);

                    // Lưu refresh token vào cookie
                    if (!string.IsNullOrEmpty(response.RefreshToken))
                    {
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = model.RememberMe ? DateTime.Now.AddDays(7) : DateTime.Now.AddHours(1)
                        };

                        Response.Cookies.Append("RefreshToken", response.RefreshToken, cookieOptions);
                    }

                    // Lưu tên người dùng vào session để hiển thị
                    HttpContext.Session.SetString("Username", model.Username);

                    _logger.LogInformation("User {Username} logged in successfully", model.Username);

                    // Chuyển hướng đến returnUrl hoặc trang chủ
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Hiển thị thông báo lỗi từ API
                    if (!string.IsNullOrEmpty(response.Message))
                    {
                        model.ErrorMessage = response.Message;
                    }
                    else
                    {
                        model.ErrorMessage = "Đăng nhập không thành công. Vui lòng kiểm tra lại thông tin đăng nhập.";
                    }

                    _logger.LogWarning("Failed login attempt for user {Username}: {Message}",
                        model.Username, response.Message);

                    // Hiển thị lại form đăng nhập với thông báo lỗi
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {Username}", model.Username);

                // Hiển thị thông báo lỗi thân thiện với người dùng
                model.ErrorMessage = "Không thể kết nối đến máy chủ. Vui lòng thử lại sau.";

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            // Clear session
            HttpContext.Session.Clear();

            // Clear cookies
            Response.Cookies.Delete("RefreshToken");

            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["RefreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return RedirectToAction("Login");
            }

            try
            {
                var response = await _authService.RefreshTokenAsync(refreshToken);

                if (response.Success && !string.IsNullOrEmpty(response.AccessToken))
                {
                    // Update access token in session
                    HttpContext.Session.SetString("Token", response.AccessToken);

                    // If a new refresh token is provided, update it in the cookie
                    if (!string.IsNullOrEmpty(response.RefreshToken))
                    {
                        var cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTime.Now.AddDays(7)
                        };

                        Response.Cookies.Append("RefreshToken", response.RefreshToken, cookieOptions);
                    }

                    // Return to the previous page or home
                    var returnUrl = Request.Headers["Referer"].ToString();
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // If refresh token is invalid, redirect to login
                    return RedirectToAction("Login");
                }
            }
            catch
            {
                return RedirectToAction("Login");
            }
        }
    }
}
