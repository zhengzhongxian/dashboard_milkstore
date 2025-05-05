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

            // Check if user is authenticated
            var token = context.Session.GetString("Token");
            
            // If no token, redirect to login
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogInformation("No token found, redirecting to login");
                context.Response.Redirect("/Account/Login");
                return;
            }

            // Check if token is expired
            if (authService.IsTokenExpired(token))
            {
                _logger.LogInformation("Token expired, attempting to refresh");
                
                // Try to refresh token
                var refreshToken = context.Request.Cookies["RefreshToken"];
                if (!string.IsNullOrEmpty(refreshToken))
                {
                    var response = await authService.RefreshTokenAsync(refreshToken);
                    
                    if (response.Success && !string.IsNullOrEmpty(response.AccessToken))
                    {
                        // Update token in session
                        context.Session.SetString("Token", response.AccessToken);
                        
                        // Update refresh token if provided
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
                        // If refresh failed, redirect to login
                        _logger.LogWarning("Token refresh failed, redirecting to login");
                        context.Response.Redirect("/Account/Login");
                        return;
                    }
                }
                else
                {
                    // No refresh token, redirect to login
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
