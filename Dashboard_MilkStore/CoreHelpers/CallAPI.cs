using System.Net.Http.Headers;
using System.Text.Json;

namespace Dashboard_MilkStore.CoreHelpers
{
    public class CallAPI
    {
        private readonly HttpClient _httpClient;

        private readonly IHttpContextAccessor? _httpContextAccessor;

        public CallAPI()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public CallAPI(IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<T?> GetAsync<T>(string url, string? token = null)
        {
            try
            {
                Console.WriteLine($"GetAsync: Starting request to {url}");

                if (!string.IsNullOrEmpty(token))
                {
                    // Không sử dụng AuthenticationHeaderValue mà thêm trực tiếp vào header
                    Console.WriteLine("GetAsync: Using provided token");
                    if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                    {
                        _httpClient.DefaultRequestHeaders.Remove("Authorization");
                    }
                    _httpClient.DefaultRequestHeaders.Add("Authorization", token);

                    // In ra header để kiểm tra
                    Console.WriteLine($"GetAsync: Authorization header: {_httpClient.DefaultRequestHeaders.GetValues("Authorization").FirstOrDefault()}");
                }
                else if (_httpContextAccessor?.HttpContext != null)
                {
                    // Tự động lấy token từ session nếu có
                    var sessionToken = _httpContextAccessor.HttpContext.Session.GetString("Token");
                    if (!string.IsNullOrEmpty(sessionToken))
                    {
                        Console.WriteLine("GetAsync: Using token from session");
                        if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                        {
                            _httpClient.DefaultRequestHeaders.Remove("Authorization");
                        }
                        _httpClient.DefaultRequestHeaders.Add("Authorization", sessionToken);

                        // In ra header để kiểm tra
                        Console.WriteLine($"GetAsync: Authorization header: {_httpClient.DefaultRequestHeaders.GetValues("Authorization").FirstOrDefault()}");
                    }
                    else
                    {
                        Console.WriteLine("GetAsync: No token found in session");
                    }
                }
                else
                {
                    Console.WriteLine("GetAsync: No token provided and no HttpContext available");
                }

                Console.WriteLine("GetAsync: Sending request");
                var response = await _httpClient.GetAsync(url);
                Console.WriteLine($"GetAsync: Response status code: {response.StatusCode}");

                // Đọc nội dung phản hồi ngay cả khi không thành công
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"GetAsync: Response content: {content}");

                // Đảm bảo response thành công
                response.EnsureSuccessStatusCode();

                return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP error in GetAsync: {ex.Message}");
                throw;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON error in GetAsync: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error in GetAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<T?> PostAsync<T>(string url, object data, string? token = null)
        {
            if (!string.IsNullOrEmpty(token))
            {
                // Không sử dụng AuthenticationHeaderValue mà thêm trực tiếp vào header
                Console.WriteLine("PostAsync: Using provided token");
                if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                {
                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                }
                _httpClient.DefaultRequestHeaders.Add("Authorization", token);

                // In ra header để kiểm tra
                Console.WriteLine($"PostAsync: Authorization header: {_httpClient.DefaultRequestHeaders.GetValues("Authorization").FirstOrDefault()}");
            }
            else if (_httpContextAccessor?.HttpContext != null)
            {
                // Tự động lấy token từ session nếu có
                var sessionToken = _httpContextAccessor.HttpContext.Session.GetString("Token");
                if (!string.IsNullOrEmpty(sessionToken))
                {
                    Console.WriteLine("PostAsync: Using token from session");
                    if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                    {
                        _httpClient.DefaultRequestHeaders.Remove("Authorization");
                    }
                    _httpClient.DefaultRequestHeaders.Add("Authorization", sessionToken);

                    // In ra header để kiểm tra
                    Console.WriteLine($"PostAsync: Authorization header: {_httpClient.DefaultRequestHeaders.GetValues("Authorization").FirstOrDefault()}");
                }
                else
                {
                    Console.WriteLine("PostAsync: No token found in session");
                }
            }
            else
            {
                Console.WriteLine("PostAsync: No token provided and no HttpContext available");
            }

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(url, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Ghi log để debug
                Console.WriteLine($"POST response from {url}: Status={response.StatusCode}, Content={responseContent}");

                // Ngay cả khi response không thành công, vẫn cố gắng deserialize
                if (!string.IsNullOrEmpty(responseContent))
                {
                    return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                // Nếu không deserialize được, đảm bảo response thành công
                response.EnsureSuccessStatusCode();
                return default;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP error in PostAsync: {ex.Message}");
                throw;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON error in PostAsync: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error in PostAsync: {ex.Message}");
                throw;
            }
        }

        public async Task<T?> PatchAsync<T>(string url, object data, string? token = null)
        {
            if (!string.IsNullOrEmpty(token))
            {
                // Không sử dụng AuthenticationHeaderValue mà thêm trực tiếp vào header
                Console.WriteLine("PatchAsync: Using provided token");
                if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                {
                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                }
                _httpClient.DefaultRequestHeaders.Add("Authorization", token);

                // In ra header để kiểm tra
                Console.WriteLine($"PatchAsync: Authorization header: {_httpClient.DefaultRequestHeaders.GetValues("Authorization").FirstOrDefault()}");
            }
            else if (_httpContextAccessor?.HttpContext != null)
            {
                // Tự động lấy token từ session nếu có
                var sessionToken = _httpContextAccessor.HttpContext.Session.GetString("Token");
                if (!string.IsNullOrEmpty(sessionToken))
                {
                    Console.WriteLine("PatchAsync: Using token from session");
                    if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                    {
                        _httpClient.DefaultRequestHeaders.Remove("Authorization");
                    }
                    _httpClient.DefaultRequestHeaders.Add("Authorization", sessionToken);

                    // In ra header để kiểm tra
                    Console.WriteLine($"PatchAsync: Authorization header: {_httpClient.DefaultRequestHeaders.GetValues("Authorization").FirstOrDefault()}");
                }
                else
                {
                    Console.WriteLine("PatchAsync: No token found in session");
                }
            }
            else
            {
                Console.WriteLine("PatchAsync: No token provided and no HttpContext available");
            }

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<T?> PutAsync<T>(string url, object data, string? token = null)
        {
            if (!string.IsNullOrEmpty(token))
            {
                // Không sử dụng AuthenticationHeaderValue mà thêm trực tiếp vào header
                Console.WriteLine("PutAsync: Using provided token");
                if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                {
                    _httpClient.DefaultRequestHeaders.Remove("Authorization");
                }
                _httpClient.DefaultRequestHeaders.Add("Authorization", token);

                // In ra header để kiểm tra
                Console.WriteLine($"PutAsync: Authorization header: {_httpClient.DefaultRequestHeaders.GetValues("Authorization").FirstOrDefault()}");
            }
            else if (_httpContextAccessor?.HttpContext != null)
            {
                // Tự động lấy token từ session nếu có
                var sessionToken = _httpContextAccessor.HttpContext.Session.GetString("Token");
                if (!string.IsNullOrEmpty(sessionToken))
                {
                    Console.WriteLine("PutAsync: Using token from session");
                    if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
                    {
                        _httpClient.DefaultRequestHeaders.Remove("Authorization");
                    }
                    _httpClient.DefaultRequestHeaders.Add("Authorization", sessionToken);

                    // In ra header để kiểm tra
                    Console.WriteLine($"PutAsync: Authorization header: {_httpClient.DefaultRequestHeaders.GetValues("Authorization").FirstOrDefault()}");
                }
                else
                {
                    Console.WriteLine("PutAsync: No token found in session");
                }
            }
            else
            {
                Console.WriteLine("PutAsync: No token provided and no HttpContext available");
            }

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(url, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}