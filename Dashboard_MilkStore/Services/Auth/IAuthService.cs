using Dashboard_MilkStore.Models.Account;

namespace Dashboard_MilkStore.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> AdminLoginAsync(string username, string password);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        bool IsTokenExpired(string token);
        bool IsRefreshTokenExpired(string refreshToken);
    }
}
