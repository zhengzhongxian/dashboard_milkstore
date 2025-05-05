using Dashboard_MilkStore.CoreHelpers;
using Dashboard_MilkStore.Models.Account;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Dashboard_MilkStore.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly CallAPI _callAPI;
        private readonly string _baseUrl;


        public AuthService(CallAPI callAPI, IConfiguration configuration)
        {
            _callAPI = callAPI;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:5000";
        }

        public async Task<AuthResponse> AdminLoginAsync(string username, string password)
        {
            try
            {
                var loginData = new
                {
                    UserName = username,
                    Password = password
                };

                var url = $"{_baseUrl}/api/Auth/admin/login";
                Console.WriteLine($"Calling API: {url}");

                // Use a more generic approach to handle the response
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var json = System.Text.Json.JsonSerializer.Serialize(loginData);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var httpResponse = await httpClient.PostAsync(url, content);
                var responseContent = await httpResponse.Content.ReadAsStringAsync();

                Console.WriteLine($"Response status: {httpResponse.StatusCode}");
                Console.WriteLine($"Response content: {responseContent}");

                // Kiểm tra trạng thái HTTP trước
                if (!httpResponse.IsSuccessStatusCode)
                {
                    // Nếu không thành công, cố gắng trích xuất thông báo lỗi từ phản hồi
                    try
                    {
                        using (JsonDocument doc = JsonDocument.Parse(responseContent))
                        {
                            // Kiểm tra nếu có trường message
                            if (doc.RootElement.TryGetProperty("message", out JsonElement messageElement))
                            {
                                string errorMessage = messageElement.GetString() ?? "Đăng nhập không thành công";
                                Console.WriteLine($"Error message from API: {errorMessage}");

                                return new AuthResponse
                                {
                                    Success = false,
                                    Message = errorMessage,
                                    StatusCode = (int)httpResponse.StatusCode
                                };
                            }

                            // Kiểm tra nếu có trường errors
                            if (doc.RootElement.TryGetProperty("errors", out JsonElement errorsElement))
                            {
                                if (errorsElement.ValueKind == JsonValueKind.Array)
                                {
                                    var errors = new List<string>();
                                    foreach (var error in errorsElement.EnumerateArray())
                                    {
                                        errors.Add(error.GetString() ?? "");
                                    }

                                    string errorMessage = string.Join(", ", errors.Where(e => !string.IsNullOrEmpty(e)));
                                    if (!string.IsNullOrEmpty(errorMessage))
                                    {
                                        Console.WriteLine($"Validation errors from API: {errorMessage}");
                                        return new AuthResponse
                                        {
                                            Success = false,
                                            Message = errorMessage,
                                            StatusCode = (int)httpResponse.StatusCode
                                        };
                                    }
                                }
                                else if (errorsElement.ValueKind == JsonValueKind.Object)
                                {
                                    var errors = new List<string>();
                                    foreach (var property in errorsElement.EnumerateObject())
                                    {
                                        if (property.Value.ValueKind == JsonValueKind.Array)
                                        {
                                            foreach (var error in property.Value.EnumerateArray())
                                            {
                                                errors.Add(error.GetString() ?? "");
                                            }
                                        }
                                    }

                                    string errorMessage = string.Join(", ", errors.Where(e => !string.IsNullOrEmpty(e)));
                                    if (!string.IsNullOrEmpty(errorMessage))
                                    {
                                        Console.WriteLine($"Validation errors from API: {errorMessage}");
                                        return new AuthResponse
                                        {
                                            Success = false,
                                            Message = errorMessage,
                                            StatusCode = (int)httpResponse.StatusCode
                                        };
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing error response: {ex.Message}");
                    }

                    // Nếu không thể trích xuất thông báo lỗi cụ thể, trả về thông báo mặc định
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Tên đăng nhập hoặc mật khẩu không đúng",
                        StatusCode = (int)httpResponse.StatusCode
                    };
                }

                // Nếu thành công, cố gắng phân tích phản hồi
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var response = System.Text.Json.JsonSerializer.Deserialize<AuthResponse>(responseContent, options);

                    if (response == null)
                    {
                        return new AuthResponse
                        {
                            Success = false,
                            Message = "Không thể kết nối đến máy chủ. Vui lòng thử lại sau.",
                            StatusCode = 500
                        };
                    }

                    return response;
                }
                catch (JsonException jsonEx)
                {
                    Console.WriteLine($"JSON deserialization error: {jsonEx.Message}");

                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Đã xảy ra lỗi khi xử lý phản hồi từ máy chủ",
                        StatusCode = (int)httpResponse.StatusCode
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AdminLoginAsync: {ex.Message}");
                return new AuthResponse
                {
                    Success = false,
                    Message = "Không thể kết nối đến máy chủ. Vui lòng thử lại sau.",
                    StatusCode = 500
                };
            }
        }

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var url = $"{_baseUrl}/api/Auth/refresh-token?refreshToken={Uri.EscapeDataString(refreshToken)}";
                Console.WriteLine($"Calling API: {url}");

                // Use a more generic approach to handle the response
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json");

                var httpResponse = await httpClient.PostAsync(url, content);
                var responseContent = await httpResponse.Content.ReadAsStringAsync();

                Console.WriteLine($"Response status: {httpResponse.StatusCode}");
                Console.WriteLine($"Response content: {responseContent}");

                // Kiểm tra trạng thái HTTP trước
                if (!httpResponse.IsSuccessStatusCode)
                {
                    // Nếu không thành công, cố gắng trích xuất thông báo lỗi từ phản hồi
                    try
                    {
                        using (JsonDocument doc = JsonDocument.Parse(responseContent))
                        {
                            // Kiểm tra nếu có trường message
                            if (doc.RootElement.TryGetProperty("message", out JsonElement messageElement))
                            {
                                string errorMessage = messageElement.GetString() ?? "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
                                Console.WriteLine($"Error message from API: {errorMessage}");

                                return new AuthResponse
                                {
                                    Success = false,
                                    Message = errorMessage,
                                    StatusCode = (int)httpResponse.StatusCode
                                };
                            }

                            // Kiểm tra nếu có trường errors
                            if (doc.RootElement.TryGetProperty("errors", out JsonElement errorsElement))
                            {
                                if (errorsElement.ValueKind == JsonValueKind.Array)
                                {
                                    var errors = new List<string>();
                                    foreach (var error in errorsElement.EnumerateArray())
                                    {
                                        errors.Add(error.GetString() ?? "");
                                    }

                                    string errorMessage = string.Join(", ", errors.Where(e => !string.IsNullOrEmpty(e)));
                                    if (!string.IsNullOrEmpty(errorMessage))
                                    {
                                        Console.WriteLine($"Validation errors from API: {errorMessage}");
                                        return new AuthResponse
                                        {
                                            Success = false,
                                            Message = errorMessage,
                                            StatusCode = (int)httpResponse.StatusCode
                                        };
                                    }
                                }
                                else if (errorsElement.ValueKind == JsonValueKind.Object)
                                {
                                    var errors = new List<string>();
                                    foreach (var property in errorsElement.EnumerateObject())
                                    {
                                        if (property.Value.ValueKind == JsonValueKind.Array)
                                        {
                                            foreach (var error in property.Value.EnumerateArray())
                                            {
                                                errors.Add(error.GetString() ?? "");
                                            }
                                        }
                                    }

                                    string errorMessage = string.Join(", ", errors.Where(e => !string.IsNullOrEmpty(e)));
                                    if (!string.IsNullOrEmpty(errorMessage))
                                    {
                                        Console.WriteLine($"Validation errors from API: {errorMessage}");
                                        return new AuthResponse
                                        {
                                            Success = false,
                                            Message = errorMessage,
                                            StatusCode = (int)httpResponse.StatusCode
                                        };
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error parsing error response: {ex.Message}");
                    }

                    // Nếu không thể trích xuất thông báo lỗi cụ thể, trả về thông báo mặc định
                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.",
                        StatusCode = (int)httpResponse.StatusCode
                    };
                }

                // Nếu thành công, cố gắng phân tích phản hồi
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var response = System.Text.Json.JsonSerializer.Deserialize<AuthResponse>(responseContent, options);

                    if (response == null)
                    {
                        return new AuthResponse
                        {
                            Success = false,
                            Message = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.",
                            StatusCode = 500
                        };
                    }

                    return response;
                }
                catch (JsonException jsonEx)
                {
                    Console.WriteLine($"JSON deserialization error: {jsonEx.Message}");

                    return new AuthResponse
                    {
                        Success = false,
                        Message = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.",
                        StatusCode = (int)httpResponse.StatusCode
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RefreshTokenAsync: {ex.Message}");
                return new AuthResponse
                {
                    Success = false,
                    Message = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.",
                    StatusCode = 500
                };
            }
        }

        public bool IsTokenExpired(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                if (!handler.CanReadToken(token))
                {
                    return true;
                }

                var jwtToken = handler.ReadJwtToken(token);
                var expiry = jwtToken.ValidTo;

                // Add a small buffer (5 minutes) to refresh token before it actually expires
                return expiry.AddMinutes(-5) < DateTime.UtcNow;
            }
            catch
            {
                return true;
            }
        }
    }
}
