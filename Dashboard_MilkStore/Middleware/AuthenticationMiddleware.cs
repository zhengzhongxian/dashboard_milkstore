using Dashboard_MilkStore.Services.Auth;

namespace Dashboard_MilkStore.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthenticationMiddleware> _logger;

        public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IAuthService authService)
        {
            // Skip authentication check for login page and static files
            var path = context.Request.Path.Value?.ToLower();
            if (path != null && (
                path.StartsWith("/account/login") ||
                path.StartsWith("/lib/") ||
                path.StartsWith("/css/") ||
                path.StartsWith("/js/") ||
                path.StartsWith("/images/")))
            {
                await _next(context);
                return;
            }

            // Kiểm tra access token trong session
            var token = context.Session.GetString("Token");
            var refreshToken = context.Request.Cookies["RefreshToken"];

            // Nếu không có access token nhưng có refresh token, kiểm tra refresh token và thử làm mới token
            if (string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(refreshToken))
            {
                // Kiểm tra refresh token có hết hạn không
                if (authService.IsRefreshTokenExpired(refreshToken))
                {
                    _logger.LogWarning("Refresh token expired, redirecting to login");
                    context.Response.Cookies.Delete("RefreshToken"); // Xóa refresh token hết hạn
                    context.Response.Redirect("/Account/Login");
                    return;
                }

                _logger.LogInformation("No access token but refresh token exists, attempting to refresh");
                var response = await authService.RefreshTokenAsync(refreshToken);

                if (response.Success && !string.IsNullOrEmpty(response.AccessToken))
                {
                    // Cập nhật access token mới vào session
                    context.Session.SetString("Token", response.AccessToken);

                    // Cập nhật refresh token mới nếu có
                    if (!string.IsNullOrEmpty(response.RefreshToken))
                    {
                        context.Response.Cookies.Append("RefreshToken", response.RefreshToken, new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true,
                            SameSite = SameSiteMode.Strict,
                            Expires = DateTime.Now.AddDays(7)
                        });
                    }

                    _logger.LogInformation("Token refreshed successfully from refresh token");
                    token = response.AccessToken; // Cập nhật token để sử dụng trong request hiện tại
                }
                else
                {
                    // Nếu làm mới thất bại, chuyển hướng đến trang đăng nhập
                    _logger.LogWarning("Token refresh failed, redirecting to login");
                    context.Response.Redirect("/Account/Login");
                    return;
                }
            }
            // Nếu không có cả access token và refresh token, chuyển hướng đến trang đăng nhập
            else if (string.IsNullOrEmpty(token))
            {
                _logger.LogInformation("No tokens found, redirecting to login");
                context.Response.Redirect("/Account/Login");
                return;
            }

            // Kiểm tra access token có hết hạn không
            if (!string.IsNullOrEmpty(token) && authService.IsTokenExpired(token))
            {
                _logger.LogInformation("Access token expired, attempting to refresh");

                // Nếu có refresh token, kiểm tra refresh token và thử làm mới access token
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    // Kiểm tra refresh token có hết hạn không
                    if (authService.IsRefreshTokenExpired(refreshToken))
                    {
                        _logger.LogWarning("Refresh token expired, redirecting to login");
                        context.Response.Cookies.Delete("RefreshToken"); // Xóa refresh token hết hạn
                        context.Response.Redirect("/Account/Login");
                        return;
                    }

                    try
                    {
                        var response = await authService.RefreshTokenAsync(refreshToken);

                        if (response.Success && !string.IsNullOrEmpty(response.AccessToken))
                        {
                            // Cập nhật access token mới vào session
                            context.Session.SetString("Token", response.AccessToken);

                            // Cập nhật refresh token mới nếu có
                            if (!string.IsNullOrEmpty(response.RefreshToken))
                            {
                                context.Response.Cookies.Append("RefreshToken", response.RefreshToken, new CookieOptions
                                {
                                    HttpOnly = true,
                                    Secure = true,
                                    SameSite = SameSiteMode.Strict,
                                    Expires = DateTime.Now.AddDays(7)
                                });
                            }

                            _logger.LogInformation("Token refreshed successfully");
                        }
                        else
                        {
                            // Nếu làm mới thất bại, chuyển hướng đến trang đăng nhập
                            _logger.LogWarning("Token refresh failed: {0}", response.Message);
                            context.Response.Redirect("/Account/Login");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Xử lý lỗi khi làm mới token
                        _logger.LogError(ex, "Error refreshing token");
                        context.Response.Redirect("/Account/Login");
                        return;
                    }
                }
                else
                {
                    // Không có refresh token, chuyển hướng đến trang đăng nhập
                    _logger.LogWarning("No refresh token found, redirecting to login");
                    context.Response.Redirect("/Account/Login");
                    return;
                }
            }

            await _next(context);
        }
    }

    // Extension method to add the middleware to the HTTP request pipeline
    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}
